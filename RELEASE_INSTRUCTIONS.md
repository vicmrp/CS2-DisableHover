# Release steps

1. Test the Release Candidate using RELEASE_TEST_CHECKLIST.md.
2. Close Cities: Skylines II before publishing to avoid locked mod files.
3. Build Release: `dotnet build -c Release`.
4. Confirm `Properties/PublishConfiguration.xml` has ModId 141603 and the desired ModVersion/ChangeLog.
5. Publish the existing mod with the `PublishNewVersion` profile from Visual Studio/Rider, or the equivalent `dotnet publish` command used by the CS2 mod toolchain.
6. Launch the game, open Paradox Mods, verify version, description/screenshots, subscribe/update it, and run a quick smoke test.
7. Check the Paradox Mods comments/logs after release for selection or transport-tool regressions.
