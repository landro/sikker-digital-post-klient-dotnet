##FUNCTIONS##

# This is an automatic variable set to the current file's/module's directory
$filePicker = "$PSScriptRoot\FilePicker.ps1";

. $filePicker.ToString();

"Du starter nå importering av sertifikat / privatnøkkel til dine personlige sertifikater"
Write-Host "Først må du velge filen. Trykk en tast for å velge"

$nøkkelpath = Get-FileName -initialDirectory "c:\"

#$nøkkelpath = ReadHost 'Hva er filstien til privatnøkkelen?'
$enteredpwd = Read-Host 'Hva er passordet til privatnøkkelen?'

$mypwd = ConvertTo-SecureString -String $enteredpwd -Force -AsPlainText
Import-PfxCertificate -FilePath "C:\Users\Aleksander Sjafjell\Desktop\buypass_mf_test_autentisering.p12" cert:\currentUser\my -Exportable -Password $mypwd  -Confirm
Write-Host Trykk en tast for å avslutte ...


