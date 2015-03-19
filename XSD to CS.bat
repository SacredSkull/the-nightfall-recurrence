@echo off
"%~dp0\xsd2code\xsd2code.exe" "%~dp0\Assets\Entities\software.xsd" SacredSkull.Software "%~dp0\Assets\Scripts\Entity\Software.xsd.cs" /pl Net35 /is /ap /xa
REM "%~dp0xsd2code\fart.exe" "%~dp0Assets\Scripts\Entity\Software.xsd.cs" "public short movement" "new public short movement"
pause