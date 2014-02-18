@echo off

setlocal

set /p configChoice=Choose your build configuration (Debug = d, Release = r? (d, r)

if /i "%configChoice:~,1%" EQU "D" set config=Debug
if /i "%configChoice:~,1%" EQU "R" set config=Release
 
packages\NUnit.Runners.2.6.3\tools\nunit-console Widespace.UnitTest\bin\%config%\Widespace.UnitTest.dll

pause
endlocal
