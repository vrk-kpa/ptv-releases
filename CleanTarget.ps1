param($target)

Remove-Item -Path $target -Recurse -Force -ErrorAction SilentlyContinue

& cmd /c "npm cache clean"
