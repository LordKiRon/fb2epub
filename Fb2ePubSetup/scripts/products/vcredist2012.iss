// requires Windows 7, Windows 7 Service Pack 1, Windows Server 2003 Service Pack 2, Windows Server 2008, Windows Server 2008 R2, Windows Server 2008 R2 SP1, Windows Vista Service Pack 1, Windows XP Service Pack 3
// requires Windows Installer 3.1 or later
// requires Internet Explorer 5.01 or later
// http://www.microsoft.com/en-us/download/details.aspx?id=30679

[CustomMessages]
vcredist2012_title=Visual C++ Redistributable for Visual Studio 2012 Update 3

en.vcredist2012_size=6.2 MB
ru.vcredist2012_size=6,2 MB

en.vcredist2012_size_x64=6.9 MB
ru.vcredist2012_size_x64=6,9 MB

;http://www.microsoft.com/globaldev/reference/lcid-all.mspx
en.vcredist2012_lcid=
ru.vcredist2012_lcid=/lcid 1049


[Code]
const
//11.0.51106 (Update1)
  VC_2012_REDIST_X86_UP1 = '{8e70e4e1-06d7-470b-9f74-a51bef21088e}';
  VC_2012_REDIST_X64_UP1 = '{6e8f74e0-43bd-4dce-8477-6ff6828acc07}';
//11.0.60610 (Update3)
  VC_2012_REDIST_X86_UP3 = '{95716cce-fc71-413f-8ad5-56c2892d4b3a}';
  VC_2012_REDIST_X64_UP3 = '{a1909659-0a08-4554-8af1-2175904903a1}';
 
  UNINSTALL_PATH = 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\';

function VCVersionInstalled(const ProductID: string): Boolean;
var
Installed: cardinal;
begin
  RegQueryDWordValue(HKEY_LOCAL_MACHINE_32,UNINSTALL_PATH + ProductID ,'Installed',Installed);
  Result := (Installed = 1);
end;

function VCRedistNeedsInstall86: Boolean;
begin
    // here the Result must be True when you need to install your VCRedist
  // or False when you don't need to, so now it's upon you how you build
  // this statement, the following won't install your VC redist only when
  Result := not (VCVersionInstalled(VC_2012_REDIST_X86_UP3) );
end;

function VCRedistNeedsInstall64: Boolean;
begin
    // here the Result must be True when you need to install your VCRedist
  // or False when you don't need to, so now it's upon you how you build
  // this statement, the following won't install your VC redist only when
  Result := (not VCVersionInstalled(VC_2012_REDIST_X64_UP3)) and IsX64();
end;


const
	vcredist2012_url = 'http://download.microsoft.com/download/1/6/B/16B06F60-3B20-4FF2-B699-5E9B7962F9AE/VSU1/vcredist_x86.exe';
	vcredist2012_url_x64 = 'http://download.microsoft.com/download/1/6/B/16B06F60-3B20-4FF2-B699-5E9B7962F9AE/VSU1/vcredist_x64.exe';
	vcredist2012_u3_url = 'http://download.microsoft.com/download/1/6/B/16B06F60-3B20-4FF2-B699-5E9B7962F9AE/VSU3/vcredist_x86.exe';
	vcredist2012_u3_url_x64 = 'http://download.microsoft.com/download/1/6/B/16B06F60-3B20-4FF2-B699-5E9B7962F9AE/VSU3/vcredist_x64.exe';

procedure vcredist2012();
begin

	if (VCRedistNeedsInstall86()) then
		AddProduct('vcredist2012' + GetArchitectureString() + '.exe',
			CustomMessage('vcredist2012_lcid') + '/passive /norestart',
			CustomMessage('vcredist2012_title')  + ' (x86)',
			CustomMessage('vcredist2012_size_x64'),
			GetString(vcredist2012_u3_url, vcredist2012_u3_url, vcredist2012_u3_url),
			false, false);
	if (VCRedistNeedsInstall64()) then
		AddProduct('vcredist2012' + '.exe',
			CustomMessage('vcredist2012_lcid') + '/passive /norestart',
			CustomMessage('vcredist2012_title')+ ' (x64)',
			CustomMessage('vcredist2012_size'),
			GetString(vcredist2012_u3_url_x64, vcredist2012_u3_url_x64, vcredist2012_u3_url_x64),
			false, false);

end;