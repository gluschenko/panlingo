# Agent instructions

## Repository skills

Repository-specific skills are stored under `.agents/skills/`.

For Panlingo language-identification work, read the complete skill instructions at:

`.agents/skills/panlingo/SKILL.md`

The skill's supporting references are in `.agents/skills/panlingo/references/`. Read the relevant reference when choosing or integrating a detector:

- `package-selection.md` for detector selection;
- `cld2.md`, `cld3.md`, `fasttext.md`, `whatlang.md`, `mediapipe.md`, and `lingua.md` for model-specific guidance;
- `language-code.md` for language-code normalization;
- `integration-patterns.md` for production integration and testing patterns.

## What the Panlingo skill covers

The skill explains how to integrate Panlingo language-detection libraries into .NET applications. It covers detector selection, model loading, platform checks, unmanaged-resource disposal, thread-safety, output normalization, adapters, and validation with real and malformed text.

Before changing language-identification code, follow the skill's workflow and preserve the repository's existing API and language-code contract.
