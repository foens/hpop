/******************************************************************************
	Copyright 2003-2004 Hamid Qureshi and Unruled Boy 
	OpenPOP.Net is free software; you can redistribute it and/or modify
	it under the terms of the Lesser GNU General Public License as published by
	the Free Software Foundation; either version 2 of the License, or
	(at your option) any later version.

	OpenPOP.Net is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	Lesser GNU General Public License for more details.

	You should have received a copy of the Lesser GNU General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
/*******************************************************************************/

/*
*Name:			OpenPOP.MIMETypes
*Function:		MIME Types
*Author:		Unruled Boy
*Created:		2004/3/29
*Modified:		2004/3/30 09:10 GMT+8
*Description:
*Changes:		
*				2004/3/30 09:10 GMT+8 by Unruled Boy
*					1.Adding full list of MIME Types
*/
using System;
using System.IO;
using System.Collections;

namespace OpenPOP
{
	/// <summary>
	/// MIMETypes
	/// </summary>
	public class MIMETypes
	{
		public const string MIMEType_MSTNEF="application/ms-tnef";
		private const string Content_Transfer_Encoding_Tag="Content-Transfer-Encoding";
		private static Hashtable _MIMETypeList=null;


		public static string GetContentTransferEncoding(string strBuffer, int pos)
		{
			int begin=0,end=0;
			begin=strBuffer.ToLower().IndexOf(Content_Transfer_Encoding_Tag.ToLower(),pos);
			if(begin!=-1)
			{
				end=strBuffer.ToLower().IndexOf("\r\n".ToLower(),begin+1);
				return strBuffer.Substring(begin+Content_Transfer_Encoding_Tag.Length+1,end-begin-Content_Transfer_Encoding_Tag.Length).Trim();
			}
			else
				return "";
		}

		public static bool IsMSTNEF(string strContentType)
		{
			if(strContentType!=null & strContentType!="")
				if(strContentType.ToLower() == MIMEType_MSTNEF.ToLower())
					return true;
				else
					return false;
			else
				return false;
		}

		public static string ContentType(string strExtension)
		{
			if(_MIMETypeList.ContainsKey(strExtension))
				return _MIMETypeList[strExtension].ToString();
			else
				return null;
		}
		public static Hashtable MIMETypeList
		{
			get{return _MIMETypeList;}
			set{_MIMETypeList=value;}
		}

		~MIMETypes()
		{
			_MIMETypeList.Clear();
			_MIMETypeList=null;
		}

		public MIMETypes()
		{
			_MIMETypeList.Add(".323","text/h323");
			_MIMETypeList.Add(".3gp","video/3gpp");
			_MIMETypeList.Add(".3gpp","video/3gpp");
			_MIMETypeList.Add(".acp","audio/x-mei-aac");
			_MIMETypeList.Add(".act","text/xml");
			_MIMETypeList.Add(".actproj","text/plain");
			_MIMETypeList.Add(".ade","application/msaccess");
			_MIMETypeList.Add(".adp","application/msaccess");
			_MIMETypeList.Add(".ai","application/postscript");
			_MIMETypeList.Add(".aif","audio/aiff");
			_MIMETypeList.Add(".aifc","audio/aiff");
			_MIMETypeList.Add(".aiff","audio/aiff");
			_MIMETypeList.Add(".asf","video/x-ms-asf");
			_MIMETypeList.Add(".asm","text/plain");
			_MIMETypeList.Add(".asx","video/x-ms-asf");
			_MIMETypeList.Add(".au","audio/basic");
			_MIMETypeList.Add(".avi","video/avi");
			_MIMETypeList.Add(".bmp","image/bmp");
			_MIMETypeList.Add(".bwp","application/x-bwpreview");
			_MIMETypeList.Add(".c","text/plain");
			_MIMETypeList.Add(".cat","application/vnd.ms-pki.seccat");
			_MIMETypeList.Add(".cc","text/plain");
			_MIMETypeList.Add(".cdf","application/x-cdf");
			_MIMETypeList.Add(".cer","application/x-x509-ca-cert");
			_MIMETypeList.Add(".cod","text/plain");
			_MIMETypeList.Add(".cpp","text/plain");
			_MIMETypeList.Add(".crl","application/pkix-crl");
			_MIMETypeList.Add(".crt","application/x-x509-ca-cert");
			_MIMETypeList.Add(".cs","text/plain");
			_MIMETypeList.Add(".css","text/css");
			_MIMETypeList.Add(".csv","application/vnd.ms-excel");
			_MIMETypeList.Add(".cxx","text/plain");
			_MIMETypeList.Add(".dbs","text/plain");
			_MIMETypeList.Add(".def","text/plain");
			_MIMETypeList.Add(".der","application/x-x509-ca-cert");
			_MIMETypeList.Add(".dib","image/bmp");
			_MIMETypeList.Add(".dif","video/x-dv");
			_MIMETypeList.Add(".dll","application/x-msdownload");
			_MIMETypeList.Add(".doc","application/msword");
			_MIMETypeList.Add(".dot","application/msword");
			_MIMETypeList.Add(".dsp","text/plain");
			_MIMETypeList.Add(".dsw","text/plain");
			_MIMETypeList.Add(".dv","video/x-dv");
			_MIMETypeList.Add(".edn","application/vnd.adobe.edn");
			_MIMETypeList.Add(".eml","message/rfc822");
			_MIMETypeList.Add(".eps","application/postscript");
			_MIMETypeList.Add(".etd","application/x-ebx");
			_MIMETypeList.Add(".etp","text/plain");
			_MIMETypeList.Add(".exe","application/x-msdownload");
			_MIMETypeList.Add(".ext","text/plain");
			_MIMETypeList.Add(".fdf","application/vnd.fdf");
			_MIMETypeList.Add(".fif","application/fractals");
			_MIMETypeList.Add(".fky","text/plain");
			_MIMETypeList.Add(".gif","image/gif");
			_MIMETypeList.Add(".gz","application/x-gzip");
			_MIMETypeList.Add(".h","text/plain");
			_MIMETypeList.Add(".hpp","text/plain");
			_MIMETypeList.Add(".hqx","application/mac-binhex40");
			_MIMETypeList.Add(".hta","application/hta");
			_MIMETypeList.Add(".htc","text/x-component");
			_MIMETypeList.Add(".htm","text/html");
			_MIMETypeList.Add(".html","text/html");
			_MIMETypeList.Add(".htt","text/webviewhtml");
			_MIMETypeList.Add(".hxx","text/plain");
			_MIMETypeList.Add(".i","text/plain");
			_MIMETypeList.Add(".iad","application/x-iad");
			_MIMETypeList.Add(".ico","image/x-icon");
			_MIMETypeList.Add(".ics","text/calendar");
			_MIMETypeList.Add(".idl","text/plain");
			_MIMETypeList.Add(".iii","application/x-iphone");
			_MIMETypeList.Add(".inc","text/plain");
			_MIMETypeList.Add(".infopathxml","application/ms-infopath.xml");
			_MIMETypeList.Add(".inl","text/plain");
			_MIMETypeList.Add(".ins","application/x-internet-signup");
			_MIMETypeList.Add(".iqy","text/x-ms-iqy");
			_MIMETypeList.Add(".isp","application/x-internet-signup");
			_MIMETypeList.Add(".java","text/java");
			_MIMETypeList.Add(".jfif","image/jpeg");
			_MIMETypeList.Add(".jnlp","application/x-java-jnlp-file");
			_MIMETypeList.Add(".jpe","image/jpeg");
			_MIMETypeList.Add(".jpeg","image/jpeg");
			_MIMETypeList.Add(".jpg","image/jpeg");
			_MIMETypeList.Add(".jsl","text/plain");
			_MIMETypeList.Add(".kci","text/plain");
			_MIMETypeList.Add(".la1","audio/x-liquid-file");
			_MIMETypeList.Add(".lar","application/x-laplayer-reg");
			_MIMETypeList.Add(".latex","application/x-latex");
			_MIMETypeList.Add(".lavs","audio/x-liquid-secure");
			_MIMETypeList.Add(".lgn","text/plain");
			_MIMETypeList.Add(".lmsff","audio/x-la-lms");
			_MIMETypeList.Add(".lqt","audio/x-la-lqt");
			_MIMETypeList.Add(".lst","text/plain");
			_MIMETypeList.Add(".m1v","video/mpeg");
			_MIMETypeList.Add(".m3u","audio/mpegurl");
			_MIMETypeList.Add(".m4e","video/mpeg4");
			_MIMETypeList.Add(".MAC","image/x-macpaint");
			_MIMETypeList.Add(".mak","text/plain");
			_MIMETypeList.Add(".man","application/x-troff-man");
			_MIMETypeList.Add(".map","text/plain");
			_MIMETypeList.Add(".mda","application/msaccess");
			_MIMETypeList.Add(".mdb","application/msaccess");
			_MIMETypeList.Add(".mde","application/msaccess");
			_MIMETypeList.Add(".mdi","image/vnd.ms-modi");
			_MIMETypeList.Add(".mfp","application/x-shockwave-flash");
			_MIMETypeList.Add(".mht","message/rfc822");
			_MIMETypeList.Add(".mhtml","message/rfc822");
			_MIMETypeList.Add(".mid","audio/mid");
			_MIMETypeList.Add(".midi","audio/mid");
			_MIMETypeList.Add(".mk","text/plain");
			_MIMETypeList.Add(".mnd","audio/x-musicnet-download");
			_MIMETypeList.Add(".mns","audio/x-musicnet-stream");
			_MIMETypeList.Add(".MP1","audio/mp1");
			_MIMETypeList.Add(".mp2","video/mpeg");
			_MIMETypeList.Add(".mp2v","video/mpeg");
			_MIMETypeList.Add(".mp3","audio/mpeg");
			_MIMETypeList.Add(".mp4","video/mp4");
			_MIMETypeList.Add(".mpa","video/mpeg");
			_MIMETypeList.Add(".mpe","video/mpeg");
			_MIMETypeList.Add(".mpeg","video/mpeg");
			_MIMETypeList.Add(".mpf","application/vnd.ms-mediapackage");
			_MIMETypeList.Add(".mpg","video/mpeg");
			_MIMETypeList.Add(".mpg4","video/mp4");
			_MIMETypeList.Add(".mpga","audio/rn-mpeg");
			_MIMETypeList.Add(".mpv2","video/mpeg");
			_MIMETypeList.Add(".NMW","application/nmwb");
			_MIMETypeList.Add(".nws","message/rfc822");
			_MIMETypeList.Add(".odc","text/x-ms-odc");
			_MIMETypeList.Add(".odh","text/plain");
			_MIMETypeList.Add(".odl","text/plain");
			_MIMETypeList.Add(".p10","application/pkcs10");
			_MIMETypeList.Add(".p12","application/x-pkcs12");
			_MIMETypeList.Add(".p7b","application/x-pkcs7-certificates");
			_MIMETypeList.Add(".p7c","application/pkcs7-mime");
			_MIMETypeList.Add(".p7m","application/pkcs7-mime");
			_MIMETypeList.Add(".p7r","application/x-pkcs7-certreqresp");
			_MIMETypeList.Add(".p7s","application/pkcs7-signature");
			_MIMETypeList.Add(".PCT","image/pict");
			_MIMETypeList.Add(".pdf","application/pdf");
			_MIMETypeList.Add(".pdx","application/vnd.adobe.pdx");
			_MIMETypeList.Add(".pfx","application/x-pkcs12");
			_MIMETypeList.Add(".pic","image/pict");
			_MIMETypeList.Add(".PICT","image/pict");
			_MIMETypeList.Add(".pko","application/vnd.ms-pki.pko");
			_MIMETypeList.Add(".png","image/png");
			_MIMETypeList.Add(".pnt","image/x-macpaint");
			_MIMETypeList.Add(".pntg","image/x-macpaint");
			_MIMETypeList.Add(".pot","application/vnd.ms-powerpoint");
			_MIMETypeList.Add(".ppa","application/vnd.ms-powerpoint");
			_MIMETypeList.Add(".pps","application/vnd.ms-powerpoint");
			_MIMETypeList.Add(".ppt","application/vnd.ms-powerpoint");
			_MIMETypeList.Add(".prc","text/plain");
			_MIMETypeList.Add(".prf","application/pics-rules");
			_MIMETypeList.Add(".ps","application/postscript");
			_MIMETypeList.Add(".pub","application/vnd.ms-publisher");
			_MIMETypeList.Add(".pwz","application/vnd.ms-powerpoint");
			_MIMETypeList.Add(".qt","video/quicktime");
			_MIMETypeList.Add(".qti","image/x-quicktime");
			_MIMETypeList.Add(".qtif","image/x-quicktime");
			_MIMETypeList.Add(".qtl","application/x-quicktimeplayer");
			_MIMETypeList.Add(".qup","application/x-quicktimeupdater");
			_MIMETypeList.Add(".r1m","application/vnd.rn-recording");
			_MIMETypeList.Add(".r3t","text/vnd.rn-realtext3d");
			_MIMETypeList.Add(".RA","audio/vnd.rn-realaudio");
			_MIMETypeList.Add(".RAM","audio/x-pn-realaudio");
			_MIMETypeList.Add(".rat","application/rat-file");
			_MIMETypeList.Add(".rc","text/plain");
			_MIMETypeList.Add(".rc2","text/plain");
			_MIMETypeList.Add(".rct","text/plain");
			_MIMETypeList.Add(".rec","application/vnd.rn-recording");
			_MIMETypeList.Add(".rgs","text/plain");
			_MIMETypeList.Add(".rjs","application/vnd.rn-realsystem-rjs");
			_MIMETypeList.Add(".rjt","application/vnd.rn-realsystem-rjt");
			_MIMETypeList.Add(".RM","application/vnd.rn-realmedia");
			_MIMETypeList.Add(".rmf","application/vnd.adobe.rmf");
			_MIMETypeList.Add(".rmi","audio/mid");
			_MIMETypeList.Add(".RMJ","application/vnd.rn-realsystem-rmj");
			_MIMETypeList.Add(".RMM","audio/x-pn-realaudio");
			_MIMETypeList.Add(".rms","application/vnd.rn-realmedia-secure");
			_MIMETypeList.Add(".rmvb","application/vnd.rn-realmedia-vbr");
			_MIMETypeList.Add(".RMX","application/vnd.rn-realsystem-rmx");
			_MIMETypeList.Add(".RNX","application/vnd.rn-realplayer");
			_MIMETypeList.Add(".rp","image/vnd.rn-realpix");
			_MIMETypeList.Add(".RPM","audio/x-pn-realaudio-plugin");
			_MIMETypeList.Add(".rqy","text/x-ms-rqy");
			_MIMETypeList.Add(".rsml","application/vnd.rn-rsml");
			_MIMETypeList.Add(".rt","text/vnd.rn-realtext");
			_MIMETypeList.Add(".rtf","application/msword");
			_MIMETypeList.Add(".rul","text/plain");
			_MIMETypeList.Add(".RV","video/vnd.rn-realvideo");
			_MIMETypeList.Add(".s","text/plain");
			_MIMETypeList.Add(".sc2","application/schdpl32");
			_MIMETypeList.Add(".scd","application/schdpl32");
			_MIMETypeList.Add(".sch","application/schdpl32");
			_MIMETypeList.Add(".sct","text/scriptlet");
			_MIMETypeList.Add(".sd2","audio/x-sd2");
			_MIMETypeList.Add(".sdp","application/sdp");
			_MIMETypeList.Add(".sit","application/x-stuffit");
			_MIMETypeList.Add(".slk","application/vnd.ms-excel");
			_MIMETypeList.Add(".sln","application/octet-stream");
			_MIMETypeList.Add(".SMI","application/smil");
			_MIMETypeList.Add(".smil","application/smil");
			_MIMETypeList.Add(".snd","audio/basic");
			_MIMETypeList.Add(".snp","application/msaccess");
			_MIMETypeList.Add(".spc","application/x-pkcs7-certificates");
			_MIMETypeList.Add(".spl","application/futuresplash");
			_MIMETypeList.Add(".sql","text/plain");
			_MIMETypeList.Add(".srf","text/plain");
			_MIMETypeList.Add(".ssm","application/streamingmedia");
			_MIMETypeList.Add(".sst","application/vnd.ms-pki.certstore");
			_MIMETypeList.Add(".stl","application/vnd.ms-pki.stl");
			_MIMETypeList.Add(".swf","application/x-shockwave-flash");
			_MIMETypeList.Add(".tab","text/plain");
			_MIMETypeList.Add(".tar","application/x-tar");
			_MIMETypeList.Add(".tdl","text/xml");
			_MIMETypeList.Add(".tgz","application/x-compressed");
			_MIMETypeList.Add(".tif","image/tiff");
			_MIMETypeList.Add(".tiff","image/tiff");
			_MIMETypeList.Add(".tlh","text/plain");
			_MIMETypeList.Add(".tli","text/plain");
			_MIMETypeList.Add(".torrent","application/x-bittorrent");
			_MIMETypeList.Add(".trg","text/plain");
			_MIMETypeList.Add(".txt","text/plain");
			_MIMETypeList.Add(".udf","text/plain");
			_MIMETypeList.Add(".udt","text/plain");
			_MIMETypeList.Add(".uls","text/iuls");
			_MIMETypeList.Add(".user","text/plain");
			_MIMETypeList.Add(".usr","text/plain");
			_MIMETypeList.Add(".vb","text/plain");
			_MIMETypeList.Add(".vcf","text/x-vcard");
			_MIMETypeList.Add(".vcproj","text/plain");
			_MIMETypeList.Add(".viw","text/plain");
			_MIMETypeList.Add(".vpg","application/x-vpeg005");
			_MIMETypeList.Add(".vspscc","text/plain");
			_MIMETypeList.Add(".vsscc","text/plain");
			_MIMETypeList.Add(".vssscc","text/plain");
			_MIMETypeList.Add(".wav","audio/wav");
			_MIMETypeList.Add(".wax","audio/x-ms-wax");
			_MIMETypeList.Add(".wbk","application/msword");
			_MIMETypeList.Add(".wiz","application/msword");
			_MIMETypeList.Add(".wm","video/x-ms-wm");
			_MIMETypeList.Add(".wma","audio/x-ms-wma");
			_MIMETypeList.Add(".wmd","application/x-ms-wmd");
			_MIMETypeList.Add(".wmv","video/x-ms-wmv");
			_MIMETypeList.Add(".wmx","video/x-ms-wmx");
			_MIMETypeList.Add(".wmz","application/x-ms-wmz");
			_MIMETypeList.Add(".wpl","application/vnd.ms-wpl");
			_MIMETypeList.Add(".wprj","application/webzip");
			_MIMETypeList.Add(".wsc","text/scriptlet");
			_MIMETypeList.Add(".wvx","video/x-ms-wvx");
			_MIMETypeList.Add(".XBM","image/x-xbitmap");
			_MIMETypeList.Add(".xdp","application/vnd.adobe.xdp+xml");
			_MIMETypeList.Add(".xfd","application/vnd.adobe.xfd+xml");
			_MIMETypeList.Add(".xfdf","application/vnd.adobe.xfdf");
			_MIMETypeList.Add(".xla","application/vnd.ms-excel");
			_MIMETypeList.Add(".xlb","application/vnd.ms-excel");
			_MIMETypeList.Add(".xlc","application/vnd.ms-excel");
			_MIMETypeList.Add(".xld","application/vnd.ms-excel");
			_MIMETypeList.Add(".xlk","application/vnd.ms-excel");
			_MIMETypeList.Add(".xll","application/vnd.ms-excel");
			_MIMETypeList.Add(".xlm","application/vnd.ms-excel");
			_MIMETypeList.Add(".xls","application/vnd.ms-excel");
			_MIMETypeList.Add(".xlt","application/vnd.ms-excel");
			_MIMETypeList.Add(".xlv","application/vnd.ms-excel");
			_MIMETypeList.Add(".xlw","application/vnd.ms-excel");
			_MIMETypeList.Add(".xml","text/xml");
			_MIMETypeList.Add(".xpl","audio/scpls");
			_MIMETypeList.Add(".xsl","text/xml");
			_MIMETypeList.Add(".z","application/x-compress");
			_MIMETypeList.Add(".zip","application/x-zip-compressed");
		}

		/// <summary>Returns the MIME content-type for the supplied file extension</summary>
		/// <returns>string MIME type (Example: \"text/plain\")</returns>
		public static string GetMimeType(string strFileName)
		{			
			try
			{
				string strFileExtension=new FileInfo(strFileName).Extension;
				string strContentType=null;
				bool MONO=false;

				if(MONO)
				{
					strContentType=MIMETypes.ContentType(strFileExtension);
				}
				else
				{
					Microsoft.Win32.RegistryKey extKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(strFileExtension);
					strContentType = (string)extKey.GetValue("Content Type");
				}

				if (strContentType.ToString() != null)
				{	
					return strContentType.ToString(); 
				}
				else
				{ return "application/octet-stream"; }
			}
			catch(System.Exception)
			{ return "application/octet-stream"; }
		}

	}
}

