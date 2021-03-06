// ProgressDialog.cpp : Implementation of CProgressDialog

#include "stdafx.h"
#include "ProgressDialog.h"
#include "Fb2EpubShlExt.h"
#include "dllmain.h"
#include <iostream>


extern class CFb2EpubExtModule _AtlModule;

using namespace std;

// CProgressDialog
//void CProgressDialog::DoWork()
//{
//	BOOL bExit = m_bExit;
//
//	while(!bExit)
//	{
//
//
//		bExit = m_bExit;
//	}
//
//	//if ( m_plsFiles == NULL || m_plsFiles->empty()  )
//	//{
//	//	return;
//	//}
//
//	//TCHAR title[256];
//	//::ZeroMemory(title,sizeof(TCHAR)*256);
//
//	//// Setup progress bar
//	//HWND hProgress = GetDlgItem(IDC_PROGRESS1);
//	//if ( ::IsWindow(hProgress) )
//	//{
//	//	SendMessage(hProgress,PBM_SETRANGE,0,MAKELPARAM(0,m_plsFiles->size()));
//	//	SendMessage(hProgress,PBM_SETPOS,0,0);
//	//	SendMessage(hProgress,PBM_SETSTEP,1,0);
//	//}
//
//	//HWND hwndText = GetDlgItem(IDC_FILE_NAME);
//	//if ( ::IsWindow(hwndText) )
//	//{
//	//	const TCHAR* emptyText = _T("");
//	//	SendMessage(hwndText,WM_SETTEXT, 0, (LPARAM)(LPCTSTR)emptyText);
//	//}
//
//	//int count = 0;
//	//USES_CONVERSION;
//	//// for all files in list - convert them
//	//for ( string_list::iterator t = m_plsFiles->begin(); t != m_plsFiles->end(); t++)
//	//{
//	//	count++;
//	//	if ( ::IsWindow(hwndText) )
//	//	{
//	//		SendMessage(hwndText,WM_SETTEXT, 0, (LPARAM)(LPCTSTR)t->c_str());
//	//	}
//
//	//	wsprintf(title,_T("Converting: %d of %d"),count,m_plsFiles->size());
//	//	SendMessage(WM_SETTEXT, 0, (LPARAM)(LPCTSTR)title);
//
//	//	LPCTSTR OutFileFolder = m_szOutPath;
//	//	if ( OutFileFolder == NULL )
//	//	{
//	//		OutFileFolder = _AtlModule.GetPathWithoutFileName((LPTSTR)t->c_str());
//	//	}
//	//	size_t outLength = lstrlen(OutFileFolder);
//	//	bool bAddSlash = false;
//	//	if ( outLength >= 2 )
//	//	{
//	//		clog << "OutFolder:" << W2A(OutFileFolder) << endl;
//	//		TCHAR lastChar = OutFileFolder[outLength-1];
//	//		if ( lastChar != '\\' || lastChar != '/' )
//	//		{
//	//			bAddSlash = true;
//	//		}
//	//	}
//	//	size_t size = t->length() + outLength + 17 + _AtlModule.m_Fb2EpubApplication.length();
//	//	TCHAR* parameter = new TCHAR[size];
//	//	ZeroMemory(parameter,size*sizeof(TCHAR));
//	//	lstrcat(parameter,_T("\""));
//	//	lstrcat(parameter,_AtlModule.m_Fb2EpubApplication.c_str());
//	//	lstrcat(parameter,_T("\""));
//	//	lstrcat(parameter,_T(" "));
//	//	lstrcat(parameter,_T("\""));
//	//	lstrcat(parameter,t->c_str());
//	//	lstrcat(parameter,_T("\""));
//	//	lstrcat(parameter,_T(" \"/o:"));
//	//	lstrcat(parameter,OutFileFolder);
//	//	if ( bAddSlash )
//	//	{
//	//		lstrcat(parameter,_T("\\"));
//	//	}
//	//	lstrcat(parameter,_T("\""));
//
//	//	PROCESS_INFORMATION procInfo;
//	//	::ZeroMemory(&procInfo,sizeof(PROCESS_INFORMATION));
//
//	//	STARTUPINFO StartupInfo;
//	//	::ZeroMemory(&StartupInfo,sizeof(STARTUPINFO));
//	//	StartupInfo.cb = sizeof(STARTUPINFO);
//	//	StartupInfo.dwFlags = STARTF_USESHOWWINDOW;
//	//	StartupInfo.wShowWindow = SW_HIDE;
//
//	//	clog << "Converting file " << W2A(t->c_str()) << " to " << W2A(OutFileFolder) << "parameter: " << W2A(parameter) << endl;
//	//	if ( CreateProcess(_AtlModule.m_Fb2EpubApplication.c_str(),parameter,NULL,NULL,FALSE,0,NULL,NULL,&StartupInfo,&procInfo) )
//	//	{
//	//		WaitForSingleObject(procInfo.hProcess,INFINITE);
//	//		CloseHandle(procInfo.hProcess);
//	//	}
//	//	else
//	//	{
//	//		int iError = GetLastError();
//	//		clog << "Unable to execute conversion process " << W2A(_AtlModule.m_Fb2EpubApplication.c_str()) << ", error " << iError << " encountered";
//	//		ATLASSERT(FALSE);
//	//	}
//	//	delete []parameter;
//	//	// advance progress bar
//	//	if ( ::IsWindow(hProgress) )
//	//	{
//	//		SendMessage(hProgress,PBM_STEPIT,0,0);
//	//	}
//	//}
//	EndDialog(0);
//}


LRESULT CProgressDialog::OnInitDialog(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
	CAxDialogImpl<CProgressDialog>::OnInitDialog(uMsg, wParam, lParam, bHandled);
	bHandled = TRUE;
	// Process all the files in the string list passed in to our constructor.
	//_beginthread(RunThread,0,this);
	return 1;  // Let the system set the focus
}


//void CProgressDialog::RunThread(void* pObject)
//{
//	CProgressDialog *pDlg = (CProgressDialog*)pObject;
//	if ( pDlg != NULL)
//	{
//		pDlg->DoWork();
//	}
//}


void CProgressDialog::Start(HWND hwndParent, long total)
{
		Create(hwndParent,NULL);
		SendMessage(UM_INIT,(WPARAM)total);
}


void CProgressDialog::AdvancePhase(BSTR bstrFileName)
{
	SendMessage(UM_ADVANCE,(WPARAM)bstrFileName);
}

void CProgressDialog::AbortPhase()
{
	SendMessage(UM_SKIP);
}

LRESULT CProgressDialog::OnLocalInit(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
	bHandled = TRUE;
	m_total = (long)wParam;

	m_total*=3;

	TCHAR title[256];
	::ZeroMemory(title,sizeof(TCHAR)*256);

	HWND hProgress = GetDlgItem(IDC_PROGRESS1);

	// Setup progress barHWND hProgress = GetDlgItem(IDC_PROGRESS1);
	if ( ::IsWindow(hProgress) )
	{
		SendMessage(hProgress,PBM_SETRANGE32,0,m_total);
		SendMessage(hProgress,PBM_SETPOS,0,0);
		SendMessage(hProgress,PBM_SETSTEP,1,0);
	}

	HWND hwndText = GetDlgItem(IDC_FILE_NAME);
	if ( ::IsWindow(hwndText) )
	{
		const TCHAR* emptyText = _T("");
		SendMessage(hwndText,WM_SETTEXT, 0, (LPARAM)(LPCTSTR)emptyText);
	}

	wsprintf(title,L"fb2->ePub: Converting %d file(s)",(long)wParam);
	SendMessage(WM_SETTEXT, 0, (LPARAM)(LPCTSTR)title);
	ShowWindow(SW_SHOW);
	UpdateWindow();
	return 1;
}

void vout(wchar_t *wcs, size_t n, wchar_t *fmt, ...)
{
   va_list arg_ptr;
 
   va_start(arg_ptr, fmt);
   vswprintf(wcs, n, fmt, arg_ptr);
   va_end(arg_ptr);
   return;
}


LRESULT CProgressDialog::OnLocalAdvance(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
	bHandled = TRUE;
	//MessageBox(L"Here",L"Info",MB_OK);
	HWND hProgress = GetDlgItem(IDC_PROGRESS1);
	if ( ::IsWindow(hProgress) )
	{
		SendMessage(hProgress,PBM_STEPIT,0,0);

	}
	HWND hwndText = GetDlgItem(IDC_FILE_NAME);
	if ( ::IsWindow(hwndText) )
	{
		basic_string<wchar_t> fileName = (BSTR)wParam;
		SendMessage(hwndText,WM_SETTEXT, 0, (LPARAM)fileName.c_str());
	}


	return 1;
}

LRESULT CProgressDialog::OnLocalSkip(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
	bHandled = TRUE;
	HWND hProgress = GetDlgItem(IDC_PROGRESS1);
	if ( ::IsWindow(hProgress) )
	{
		// two times since we skip ProcessingSaving and Processed
		SendMessage(hProgress,PBM_STEPIT,0,0);
		SendMessage(hProgress,PBM_STEPIT,0,0);
	}
	return 1;
}

void CProgressDialog::FinishProgress()
{
	SendMessage(UM_FINISH);
}

LRESULT CProgressDialog::OnFinish(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
	bHandled = TRUE;


	HWND hProgress = GetDlgItem(IDC_PROGRESS1);
	if ( ::IsWindow(hProgress) )
	{
		SendMessage(hProgress,PBM_SETPOS,m_total,0);
	}
	return 1;
}
