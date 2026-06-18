@echo off
REM Repair the .claude\{agents,skills} links to ..\.github\{agents,skills} on Windows.
REM
REM Tries a real symlink first (needs Developer Mode or admin); if that fails,
REM falls back to a directory junction (mklink /J) which needs no privileges and
REM works fine for local directories. Either way .claude and .github share one
REM source of truth. Idempotent: safe to run repeatedly.

setlocal
set "REPO_ROOT=%~dp0.."
pushd "%REPO_ROOT%"

call :link agents
call :link skills

popd
echo Done. .claude now mirrors .github\{agents,skills}.
endlocal
exit /b 0

:link
set "NAME=%~1"
if not exist ".github\%NAME%\" (
  echo ERROR: source .github\%NAME% does not exist 1>&2
  exit /b 1
)

REM Remove any existing link/file/dir at the destination first.
if exist ".claude\%NAME%" (
  REM rmdir works for both junctions/symlinked dirs and real dirs.
  rmdir /S /Q ".claude\%NAME%" 2>nul
  if exist ".claude\%NAME%" del /F /Q ".claude\%NAME%" 2>nul
)

REM Try a real directory symlink (relative target).
mklink /D ".claude\%NAME%" "..\.github\%NAME%" >nul 2>&1
if %errorlevel%==0 (
  echo linked ^(symlink^): .claude\%NAME% -^> ..\.github\%NAME%
  exit /b 0
)

REM Fall back to a junction. Junctions require an absolute target.
mklink /J ".claude\%NAME%" "%REPO_ROOT%\.github\%NAME%" >nul 2>&1
if %errorlevel%==0 (
  echo linked ^(junction^): .claude\%NAME% -^> .github\%NAME%
  exit /b 0
)

echo ERROR: failed to link .claude\%NAME% 1>&2
exit /b 1
