##FUNCTIONS##
""

# Dette er en automatisk variabel satt til file's/module's directory
$filePicker = "$PSScriptRoot\FilePicker.ps1";

# Legge til filvelger-script
. $filePicker.ToString();

"Du starter nå importering av sertifikat / privatnøkkel til dine personlige sertifikater"
Read-Host "Først må du velge filen. Trykk en tast for å fortsette"

$nøkkelpath = Get-FileName -initialDirectory "c:\"

#$nøkkelpath = ReadHost 'Hva er filstien til privatnøkkelen?'
$enteredpwd = Read-Host 'Hva er passordet til privatnøkkelen?'

$mypwd = ConvertTo-SecureString -String $enteredpwd -Force -AsPlainText
Import-PfxCertificate -FilePath "C:\Users\Aleksander Sjafjell\Desktop\buypass_mf_test_autentisering.p12" cert:\currentUser\my -Exportable -Password $mypwd  -Confirm
Read-Host Trykk en tast for å avslutte ...


