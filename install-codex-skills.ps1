[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [string]$CodexHome = $(if ($env:CODEX_HOME) { $env:CODEX_HOME } else { Join-Path $HOME ".codex" })
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$sourceRoot = Join-Path $repoRoot ".agents\skills"
$targetRoot = Join-Path $CodexHome "skills"
$skillNames = @(
    "panlingo"
)

function Install-OrUpdateSkill {
    param(
        [Parameter(Mandatory = $true)]
        [string]$SkillName
    )

    $sourcePath = Join-Path $sourceRoot $SkillName
    $targetPath = Join-Path $targetRoot $SkillName

    if (-not (Test-Path -LiteralPath $sourcePath -PathType Container)) {
        throw "Skill source was not found: $sourcePath"
    }

    if ($PSCmdlet.ShouldProcess($targetPath, "Install or update skill '$SkillName'")) {
        if (Test-Path -LiteralPath $targetPath) {
            Remove-Item -LiteralPath $targetPath -Recurse -Force
        }

        Copy-Item -LiteralPath $sourcePath -Destination $targetPath -Recurse
        Write-Host "Updated skill '$SkillName' -> $targetPath"
    }
}

if (-not (Test-Path -LiteralPath $targetRoot -PathType Container)) {
    if ($PSCmdlet.ShouldProcess($targetRoot, "Create Codex skills directory")) {
        New-Item -ItemType Directory -Path $targetRoot -Force | Out-Null
    }
}

foreach ($skillName in $skillNames) {
    Install-OrUpdateSkill -SkillName $skillName
}

Write-Host ""
Write-Host "Done. Restart Codex if it is already running so it picks up the refreshed skills."
