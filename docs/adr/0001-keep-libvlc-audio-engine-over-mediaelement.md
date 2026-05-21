# ADR 0001 — Keep the LibVLC-backed `MediaPlayer.AudioEngine` instead of WPF `MediaElement`

- **Status:** Proposed — *pending the FLAC smoke test in Consequences §"Owed verification".* Flips to **Accepted** when that test passes; only then is this ADR eligible to be pushed into PR [#16](https://github.com/GabrielCoetzee/Media-Player/pull/16).
- **Date:** 2026-05-18
- **Deciders:** André Gabriel Coetzee
- **Supersedes / relates to:** `docs/plans/deferred/VISUALIZATIONS_PLAN.md`, `docs/plans/GAPLESS_PLAYBACK_PLAN.md`, `docs/plans/done/ENGINE_QUEUE_REMOVAL_PLAN.md`, and the *Playback control* section of `CLAUDE.md`.

---

## Context

`MediaPlayer.AudioEngine` (the `IAudioEngine` abstraction + `LibVlcAudioEngine`, LibVLCSharp-backed) was introduced in Phase 1 of the visualizations work. WPF's `MediaElement` was **fully removed** in the same phase: `MediaElementOpenedMultiValueConverter`, `MediaElementExtension`, `MediaOpenedConverterModel`, and `MediaOpenedCommand` were deleted, the View was rewired to `AlbumArtDisplay` / `vlc:VideoView`, and the ViewModel tests were retargeted to `Mock<IAudioEngine>`.

The two features that *originally* justified LibVLC are both **not shipping for now**:

- **Visualizations** — deferred (`docs/plans/deferred/VISUALIZATIONS_PLAN.md`). Needed LibVLC for a sample-accurate audio tap; `MediaElement` is a black box with no raw sample stream.
- **Gapless playback** — deferred to its own branch (`docs/plans/GAPLESS_PLAYBACK_PLAN.md`). Needed LibVLC's dual-player priming.

This raises a fair question, and the reason this ADR exists: **with both original justifications deferred, does a ~30 MB native dependency still earn its place, or is it orphaned weight that should be reverted to `MediaElement`?** The only written justification currently lives in two *deferred* plan docs — so a future reader scanning the repo cold would reasonably conclude the engine is dead weight. This ADR records the standalone rationale where it is discoverable, independent of those plans.

## Decision

**Keep `MediaPlayer.AudioEngine`. Do not revert to `MediaElement`. The deferred visualizations and gapless plans are *not* the sole — nor even the primary — justification.**

The decision stands on three legs, ordered by how concrete they are *today*:

### 1. The architecture/test seam is realized, sunk value — reverting regresses it (present, decisive)

`IAudioEngine` is not speculative infrastructure; it already shipped and already paid off:

- The ViewModel layer is unit-testable against `Mock<IAudioEngine>` (`MediaControlsViewModelTests`). With `MediaElement`, playback was a View-coupled control reachable only through converters/commands.
- That coupling cruft (`MediaElementOpenedMultiValueConverter`, `MediaElementExtension`, `MediaOpenedConverterModel`, `MediaOpenedCommand`) is **gone**, and the codebase is cleaner for it.

Reverting to `MediaElement` would *actively destroy* working tests and re-introduce View↔playback coupling. This is the single most decisive anti-revert argument and it owes nothing to the deferred plans.

### 2. FLAC robustness for the existing library (present functional requirement)

`.flac` is a first-class entry in `ApplicationSettings.SupportedFileFormats` (`MediaPlayer.Settings/Configuration/ApplicationSettings.cs:28`) and the open-file dialog filter, and the library being played contains FLAC. `MediaElement` decodes via Windows Media Foundation: FLAC support is the *OS's*, varies by Windows build/edition, and is historically flaky on FLAC seeking and length reporting specifically. LibVLC carries its own decoders — codec behavior is *ours*, consistent across machines, and robust for FLAC. (Also covers Opus / OGG / APE, which MF still does not — one `SupportedFileFormats` line away.)

> **Honesty note (per `CLAUDE.md` framework-behavior rule):** "MediaElement *can't* play FLAC" would be too strong on Windows 10 1709+/Windows 11 — it generally can. The defensible claim is *codec independence + consistent seek/duration behavior*, not "MediaElement is incapable." This leg is currently **reasoned, not verified in this app** — see Owed verification below.

### 3. Optionality preserved at zero further engine cost (forward-looking)

Several audiophile features are LibVLC-native and **categorically impossible on `MediaElement`** — keeping the engine keeps these reachable as plain feature branches with no engine rework:

- **ReplayGain** — auto loudness-leveling. Explicitly wanted "eventually" for an inconsistently-mastered FLAC library; the strongest of these.
- **Gapless playback** — design already exists (`GAPLESS_PLAYBACK_PLAN.md`), deferred, not dead.
- **Visualizations** — design already exists (`VISUALIZATIONS_PLAN.md`), deferred, not dead.
- **Output-device picker / EQ / DSP / crossfade** — all LibVLC-native; see the *Out of Scope* list in `VISUALIZATIONS_PLAN.md`.

This leg is optionality, not need. It is real (ReplayGain is genuinely wanted) but is *not* load-bearing on its own — legs 1 and 2 carry the decision; leg 3 is upside.

## Consequences

### Positive

- ViewModel layer stays unit-testable; no re-coupling of playback to the View.
- Robust, OS-independent decoding for the FLAC library; trivially extensible to Opus/OGG/APE.
- ReplayGain / gapless / visualizations remain feature-branch-reachable with no engine change.
- Single engine for audio *and* video (`vlc:VideoView` bound to `IAudioEngine.NativePlayer`).

### Negative / accepted costs

- **~30 MB of native VLC binaries** ship with the app. Accepted for a personal/non-commercial desktop app (also noted in `VISUALIZATIONS_PLAN.md` Risks).
- **LGPL** (LibVLCSharp + VLC native runtime, dynamically linked). Fine for personal/non-commercial use; revisit only if distribution shape changes.
- **Native interop surface** — threading marshaling onto the WPF dispatcher, a native-load failure mode, and LibVLCSharp.WPF airspace risk for video. The first is handled in `LibVlcAudioEngine`; the airspace risk is tracked in `VISUALIZATIONS_PLAN.md`.

### Owed verification (gates Status → Accepted)

Leg 2 (FLAC) is currently **reasoned, not verified in this app** — consistent with the still-open Phase 1 item in `VISUALIZATIONS_PLAN.md`: *"behavior parity across MP3/FLAC/WAV."* Because FLAC is now a load-bearing leg of a recorded decision, `CLAUDE.md`'s framework-behavior rule forbids asserting it as verified until it is.

**Smoke test (manual, owner: user — GUI app, needs a real `.flac` and ears):**

1. `dotnet run --project src/MediaPlayer.Shell`
2. Add a real `.flac` file from the library.
3. Confirm: it plays audibly; reported `Duration` is correct; seek works (drag the seekbar, audio follows); the track ends cleanly and the queue advances; no exceptions in the debug output.

On pass: flip this ADR's **Status to Accepted**, drop this gate, and it is then eligible to be pushed into PR #16. On fail: the FLAC leg is wrong — re-grill before this ADR is accepted.

## Alternatives considered

- **Revert to `MediaElement`.** Rejected: destroys the shipped test seam, re-couples playback to the View, regresses FLAC robustness, and permanently closes ReplayGain/gapless/visualizations. Net-negative churn even before counting the lost upside.
- **Keep the engine but document nothing (accept the legibility risk).** Rejected: leaves the only rationale in two *deferred* plan docs, guaranteeing a future "why is this here — rip it out?" — the very question that prompted this ADR.
