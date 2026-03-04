
param(
    [string]$RepoPath,
    [string]$TargetFolder
)

Write-Host "Importing repo: $RepoPath"

git remote add temp $RepoPath
git fetch temp
git merge temp/main --allow-unrelated-histories -m "Migrate $RepoPath"
git remote remove temp

Write-Host "Moving files into $TargetFolder"
New-Item -ItemType Directory -Force -Path $TargetFolder | Out-Null
git mv * $TargetFolder
