<#
    (C) 2022 Advanced Systems
    RepoManager Reference Guide
    Author: Stefan Greve
#>

# track all remote branches from the exercises repository
clone -User StefanGreve -Name exercises
track exercises -All

# open the dtui repository in a new VS Code window
code -n $(grepo dtui).Path

# clone all repositories from StefanGreve to desktop and track all branches everywhere
$desktop = [Environment]::GetFolderPath("Desktop")
clone -Path $desktop -User stefangreve -Protocoll SSH -TrackAllBranches -All

# query all repositories in the default container, and dry-run the removal command afterwards
$repos = grepo -All | select -ExpandProperty Name
$repos | rmrepo -WhatIf

# retrieve the author's email address from the last commit
$lastCommit = grepo dtui | select -ExpandProperty LastCommit
echo $lastCommit.Author.Email

# inspect the diff of the last commit in repomanager
$repomanager = Get-Repository repomanager
$lastCommit = $repomanager | select -ExpandProperty LastCommit
git --git-dir=$($repomanager.GitPath) log -n 1 $lastCommit.Hash --patch
