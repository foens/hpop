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
	public static class MIMETypes
	{
		public const string MIMEType_MSTNEF="application/ms-tnef";
		private const string Content_Transfer_Encoding_Tag="Content-Transfer-Encoding";


	    public static string GetContentTransferEncoding(string strBuffer, int pos)
		{
	        int begin = strBuffer.ToLower().IndexOf(Content_Transfer_Encoding_Tag.ToLower(),pos);
			if(begin!=-1)
			{
			    int end = strBuffer.ToLower().IndexOf("\r\n".ToLower(),begin+1);
			    return strBuffer.Substring(begin+Content_Transfer_Encoding_Tag.Length+1,end-begin-Content_Transfer_Encoding_Tag.Length).Trim();
			}

	        return "";
		}

		public static bool IsMSTNEF(string strContentType)
		{
			if(strContentType!=null & strContentType!="")
				if(strContentType.ToLower() == MIMEType_MSTNEF.ToLower())
					return true;
				else
					return false;
			
			return false;
		}

		public static string ContentType(string strExtension)
		{
			if(MIMETypeList.ContainsKey(strExtension))
				return MIMETypeList[strExtension].ToString();
			
			return null;
		}

	    public static Hashtable MIMETypeList { get; set; }

	    static ~MIMETypes()
		{
			MIMETypeList.Clear();
			MIMETypeList=null;
		}

		static MIMETypes()
		{
			MIMETypeList.Add(".323","text/h323");
			MIMETypeList.Add(".3gp","video/3gpp");
			MIMETypeList.Add(".3gpp","video/3gpp");
			MIMETypeList.Add(".acp","audio/x-mei-aac");
			MIMETypeList.Add(".act","text/xml");
			MIMETypeList.Add(".actproj","text/plain");
			MIMETypeList.Add(".ade","application/msaccess");
			MIMETypeList.Add(".adp","application/msaccess");
			MIMETypeList.Add(".ai","application/postscript");
			MIMETypeList.Add(".aif","audio/aiff");
			MIMETypeList.Add(".aifc","audio/aiff");
			MIMETypeList.Add(".aiff","audio/aiff");
			MIMETypeList.Add(".asf","video/x-ms-asf");
			MIMETypeList.Add(".asm","text/plain");
			MIMETypeList.Add(".asx","video/x-ms-asf");
			MIMETypeList.Add(".au","audio/basic");
			MIMETypeList.Add(".avi","video/avi");
			MIMETypeList.Add(".bmp","image/bmp");
			MIMETypeList.Add(".bwp","application/x-bwpreview");
			MIMETypeList.Add(".c","text/plain");
			MIMETypeList.Add(".cat","application/vnd.ms-pki.seccat");
			MIMETypeList.Add(".cc","text/plain");
			MIMETypeList.Add(".cdf","application/x-cdf");
			MIMETypeList.Add(".cer","application/x-x509-ca-cert");
			MIMETypeList.Add(".cod","text/plain");
			MIMETypeList.Add(".cpp","text/plain");
			MIMETypeList.Add(".crl","application/pkix-crl");
			MIMETypeList.Add(".crt","application/x-x509-ca-cert");
			MIMETypeList.Add(".cs","text/plain");
			MIMETypeList.Add(".css","text/css");
			MIMETypeList.Add(".csv","application/vnd.ms-excel");
			MIMETypeList.Add(".cxx","text/plain");
			MIMETypeList.Add(".dbs","text/plain");
			MIMETypeList.Add(".def","text/plain");
			MIMETypeList.Add(".der","application/x-x509-ca-cert");
			MIMETypeList.Add(".dib","image/bmp");
			MIMETypeList.Add(".dif","video/x-dv");
			MIMETypeList.Add(".dll","application/x-msdownload");
			MIMETypeList.Add(".doc","application/msword");
			MIMETypeList.Add(".dot","application/msword");
			MIMETypeList.Add(".dsp","text/plain");
			MIMETypeList.Add(".dsw","text/plain");
			MIMETypeList.Add(".dv","video/x-dv");
			MIMETypeList.Add(".edn","application/vnd.adobe.edn");
			MIMETypeList.Add(".eml","message/rfc822");
			MIMETypeList.Add(".eps","application/postscript");
			MIMETypeList.Add(".etd","application/x-ebx");
			MIMETypeList.Add(".etp","text/plain");
			MIMETypeList.Add(".exe","application/x-msdownload");
			MIMETypeList.Add(".ext","text/plain");
			MIMETypeList.Add(".fdf","application/vnd.fdf");
			MIMETypeList.Add(".fif","application/fractals");
			MIMETypeList.Add(".fky","text/plain");
			MIMETypeList.Add(".gif","image/gif");
			MIMETypeList.Add(".gz","application/x-gzip");
			MIMETypeList.Add(".h","text/plain");
			MIMETypeList.Add(".hpp","text/plain");
			MIMETypeList.Add(".hqx","application/mac-binhex40");
			MIMETypeList.Add(".hta","application/hta");
			MIMETypeList.Add(".htc","text/x-component");
			MIMETypeList.Add(".htm","text/html");
			MIMETypeList.Add(".html","text/html");
			MIMETypeList.Add(".htt","text/webviewhtml");
			MIMETypeList.Add(".hxx","text/plain");
			MIMETypeList.Add(".i","text/plain");
			MIMETypeList.Add(".iad","application/x-iad");
			MIMETypeList.Add(".ico","image/x-icon");
			MIMETypeList.Add(".ics","text/calendar");
			MIMETypeList.Add(".idl","text/plain");
			MIMETypeList.Add(".iii","application/x-iphone");
			MIMETypeList.Add(".inc","text/plain");
			MIMETypeList.Add(".infopathxml","application/ms-infopath.xml");
			MIMETypeList.Add(".inl","text/plain");
			MIMETypeList.Add(".ins","application/x-internet-signup");
			MIMETypeList.Add(".iqy","text/x-ms-iqy");
			MIMETypeList.Add(".isp","application/x-internet-signup");
			MIMETypeList.Add(".java","text/java");
			MIMETypeList.Add(".jfif","image/jpeg");
			MIMETypeList.Add(".jnlp","application/x-java-jnlp-file");
			MIMETypeList.Add(".jpe","image/jpeg");
			MIMETypeList.Add(".jpeg","image/jpeg");
			MIMETypeList.Add(".jpg","image/jpeg");
			MIMETypeList.Add(".jsl","text/plain");
			MIMETypeList.Add(".kci","text/plain");
			MIMETypeList.Add(".la1","audio/x-liquid-file");
			MIMETypeList.Add(".lar","application/x-laplayer-reg");
			MIMETypeList.Add(".latex","application/x-latex");
			MIMETypeList.Add(".lavs","audio/x-liquid-secure");
			MIMETypeList.Add(".lgn","text/plain");
			MIMETypeList.Add(".lmsff","audio/x-la-lms");
			MIMETypeList.Add(".lqt","audio/x-la-lqt");
			MIMETypeList.Add(".lst","text/plain");
			MIMETypeList.Add(".m1v","video/mpeg");
			MIMETypeList.Add(".m3u","audio/mpegurl");
			MIMETypeList.Add(".m4e","video/mpeg4");
			MIMETypeList.Add(".MAC","image/x-macpaint");
			MIMETypeList.Add(".mak","text/plain");
			MIMETypeList.Add(".man","application/x-troff-man");
			MIMETypeList.Add(".map","text/plain");
			MIMETypeList.Add(".mda","application/msaccess");
			MIMETypeList.Add(".mdb","application/msaccess");
			MIMETypeList.Add(".mde","application/msaccess");
			MIMETypeList.Add(".mdi","image/vnd.ms-modi");
			MIMETypeList.Add(".mfp","application/x-shockwave-flash");
			MIMETypeList.Add(".mht","message/rfc822");
			MIMETypeList.Add(".mhtml","message/rfc822");
			MIMETypeList.Add(".mid","audio/mid");
			MIMETypeList.Add(".midi","audio/mid");
			MIMETypeList.Add(".mk","text/plain");
			MIMETypeList.Add(".mnd","audio/x-musicnet-download");
			MIMETypeList.Add(".mns","audio/x-musicnet-stream");
			MIMETypeList.Add(".MP1","audio/mp1");
			MIMETypeList.Add(".mp2","video/mpeg");
			MIMETypeList.Add(".mp2v","video/mpeg");
			MIMETypeList.Add(".mp3","audio/mpeg");
			MIMETypeList.Add(".mp4","video/mp4");
			MIMETypeList.Add(".mpa","video/mpeg");
			MIMETypeList.Add(".mpe","video/mpeg");
			MIMETypeList.Add(".mpeg","video/mpeg");
			MIMETypeList.Add(".mpf","application/vnd.ms-mediapackage");
			MIMETypeList.Add(".mpg","video/mpeg");
			MIMETypeList.Add(".mpg4","video/mp4");
			MIMETypeList.Add(".mpga","audio/rn-mpeg");
			MIMETypeList.Add(".mpv2","video/mpeg");
			MIMETypeList.Add(".NMW","application/nmwb");
			MIMETypeList.Add(".nws","message/rfc822");
			MIMETypeList.Add(".odc","text/x-ms-odc");
			MIMETypeList.Add(".odh","text/plain");
			MIMETypeList.Add(".odl","text/plain");
			MIMETypeList.Add(".p10","application/pkcs10");
			MIMETypeList.Add(".p12","application/x-pkcs12");
			MIMETypeList.Add(".p7b","application/x-pkcs7-certificates");
			MIMETypeList.Add(".p7c","application/pkcs7-mime");
			MIMETypeList.Add(".p7m","application/pkcs7-mime");
			MIMETypeList.Add(".p7r","application/x-pkcs7-certreqresp");
			MIMETypeList.Add(".p7s","application/pkcs7-signature");
			MIMETypeList.Add(".PCT","image/pict");
			MIMETypeList.Add(".pdf","application/pdf");
			MIMETypeList.Add(".pdx","application/vnd.adobe.pdx");
			MIMETypeList.Add(".pfx","application/x-pkcs12");
			MIMETypeList.Add(".pic","image/pict");
			MIMETypeList.Add(".PICT","image/pict");
			MIMETypeList.Add(".pko","application/vnd.ms-pki.pko");
			MIMETypeList.Add(".png","image/png");
			MIMETypeList.Add(".pnt","image/x-macpaint");
			MIMETypeList.Add(".pntg","image/x-macpaint");
			MIMETypeList.Add(".pot","application/vnd.ms-powerpoint");
			MIMETypeList.Add(".ppa","application/vnd.ms-powerpoint");
			MIMETypeList.Add(".pps","application/vnd.ms-powerpoint");
			MIMETypeList.Add(".ppt","application/vnd.ms-powerpoint");
			MIMETypeList.Add(".prc","text/plain");
			MIMETypeList.Add(".prf","application/pics-rules");
			MIMETypeList.Add(".ps","application/postscript");
			MIMETypeList.Add(".pub","application/vnd.ms-publisher");
			MIMETypeList.Add(".pwz","application/vnd.ms-powerpoint");
			MIMETypeList.Add(".qt","video/quicktime");
			MIMETypeList.Add(".qti","image/x-quicktime");
			MIMETypeList.Add(".qtif","image/x-quicktime");
			MIMETypeList.Add(".qtl","application/x-quicktimeplayer");
			MIMETypeList.Add(".qup","application/x-quicktimeupdater");
			MIMETypeList.Add(".r1m","application/vnd.rn-recording");
			MIMETypeList.Add(".r3t","text/vnd.rn-realtext3d");
			MIMETypeList.Add(".RA","audio/vnd.rn-realaudio");
			MIMETypeList.Add(".RAM","audio/x-pn-realaudio");
			MIMETypeList.Add(".rat","application/rat-file");
			MIMETypeList.Add(".rc","text/plain");
			MIMETypeList.Add(".rc2","text/plain");
			MIMETypeList.Add(".rct","text/plain");
			MIMETypeList.Add(".rec","application/vnd.rn-recording");
			MIMETypeList.Add(".rgs","text/plain");
			MIMETypeList.Add(".rjs","application/vnd.rn-realsystem-rjs");
			MIMETypeList.Add(".rjt","application/vnd.rn-realsystem-rjt");
			MIMETypeList.Add(".RM","application/vnd.rn-realmedia");
			MIMETypeList.Add(".rmf","application/vnd.adobe.rmf");
			MIMETypeList.Add(".rmi","audio/mid");
			MIMETypeList.Add(".RMJ","application/vnd.rn-realsystem-rmj");
			MIMETypeList.Add(".RMM","audio/x-pn-realaudio");
			MIMETypeList.Add(".rms","application/vnd.rn-realmedia-secure");
			MIMETypeList.Add(".rmvb","application/vnd.rn-realmedia-vbr");
			MIMETypeList.Add(".RMX","application/vnd.rn-realsystem-rmx");
			MIMETypeList.Add(".RNX","application/vnd.rn-realplayer");
			MIMETypeList.Add(".rp","image/vnd.rn-realpix");
			MIMETypeList.Add(".RPM","audio/x-pn-realaudio-plugin");
			MIMETypeList.Add(".rqy","text/x-ms-rqy");
			MIMETypeList.Add(".rsml","application/vnd.rn-rsml");
			MIMETypeList.Add(".rt","text/vnd.rn-realtext");
			MIMETypeList.Add(".rtf","application/msword");
			MIMETypeList.Add(".rul","text/plain");
			MIMETypeList.Add(".RV","video/vnd.rn-realvideo");
			MIMETypeList.Add(".s","text/plain");
			MIMETypeList.Add(".sc2","application/schdpl32");
			MIMETypeList.Add(".scd","application/schdpl32");
			MIMETypeList.Add(".sch","application/schdpl32");
			MIMETypeList.Add(".sct","text/scriptlet");
			MIMETypeList.Add(".sd2","audio/x-sd2");
			MIMETypeList.Add(".sdp","application/sdp");
			MIMETypeList.Add(".sit","application/x-stuffit");
			MIMETypeList.Add(".slk","application/vnd.ms-excel");
			MIMETypeList.Add(".sln","application/octet-stream");
			MIMETypeList.Add(".SMI","application/smil");
			MIMETypeList.Add(".smil","application/smil");
			MIMETypeList.Add(".snd","audio/basic");
			MIMETypeList.Add(".snp","application/msaccess");
			MIMETypeList.Add(".spc","application/x-pkcs7-certificates");
			MIMETypeList.Add(".spl","application/futuresplash");
			MIMETypeList.Add(".sql","text/plain");
			MIMETypeList.Add(".srf","text/plain");
			MIMETypeList.Add(".ssm","application/streamingmedia");
			MIMETypeList.Add(".sst","application/vnd.ms-pki.certstore");
			MIMETypeList.Add(".stl","application/vnd.ms-pki.stl");
			MIMETypeList.Add(".swf","application/x-shockwave-flash");
			MIMETypeList.Add(".tab","text/plain");
			MIMETypeList.Add(".tar","application/x-tar");
			MIMETypeList.Add(".tdl","text/xml");
			MIMETypeList.Add(".tgz","application/x-compressed");
			MIMETypeList.Add(".tif","image/tiff");
			MIMETypeList.Add(".tiff","image/tiff");
			MIMETypeList.Add(".tlh","text/plain");
			MIMETypeList.Add(".tli","text/plain");
			MIMETypeList.Add(".torrent","application/x-bittorrent");
			MIMETypeList.Add(".trg","text/plain");
			MIMETypeList.Add(".txt","text/plain");
			MIMETypeList.Add(".udf","text/plain");
			MIMETypeList.Add(".udt","text/plain");
			MIMETypeList.Add(".uls","text/iuls");
			MIMETypeList.Add(".user","text/plain");
			MIMETypeList.Add(".usr","text/plain");
			MIMETypeList.Add(".vb","text/plain");
			MIMETypeList.Add(".vcf","text/x-vcard");
			MIMETypeList.Add(".vcproj","text/plain");
			MIMETypeList.Add(".viw","text/plain");
			MIMETypeList.Add(".vpg","application/x-vpeg005");
			MIMETypeList.Add(".vspscc","text/plain");
			MIMETypeList.Add(".vsscc","text/plain");
			MIMETypeList.Add(".vssscc","text/plain");
			MIMETypeList.Add(".wav","audio/wav");
			MIMETypeList.Add(".wax","audio/x-ms-wax");
			MIMETypeList.Add(".wbk","application/msword");
			MIMETypeList.Add(".wiz","application/msword");
			MIMETypeList.Add(".wm","video/x-ms-wm");
			MIMETypeList.Add(".wma","audio/x-ms-wma");
			MIMETypeList.Add(".wmd","application/x-ms-wmd");
			MIMETypeList.Add(".wmv","video/x-ms-wmv");
			MIMETypeList.Add(".wmx","video/x-ms-wmx");
			MIMETypeList.Add(".wmz","application/x-ms-wmz");
			MIMETypeList.Add(".wpl","application/vnd.ms-wpl");
			MIMETypeList.Add(".wprj","application/webzip");
			MIMETypeList.Add(".wsc","text/scriptlet");
			MIMETypeList.Add(".wvx","video/x-ms-wvx");
			MIMETypeList.Add(".XBM","image/x-xbitmap");
			MIMETypeList.Add(".xdp","application/vnd.adobe.xdp+xml");
			MIMETypeList.Add(".xfd","application/vnd.adobe.xfd+xml");
			MIMETypeList.Add(".xfdf","application/vnd.adobe.xfdf");
			MIMETypeList.Add(".xla","application/vnd.ms-excel");
			MIMETypeList.Add(".xlb","application/vnd.ms-excel");
			MIMETypeList.Add(".xlc","application/vnd.ms-excel");
			MIMETypeList.Add(".xld","application/vnd.ms-excel");
			MIMETypeList.Add(".xlk","application/vnd.ms-excel");
			MIMETypeList.Add(".xll","application/vnd.ms-excel");
			MIMETypeList.Add(".xlm","application/vnd.ms-excel");
			MIMETypeList.Add(".xls","application/vnd.ms-excel");
			MIMETypeList.Add(".xlt","application/vnd.ms-excel");
			MIMETypeList.Add(".xlv","application/vnd.ms-excel");
			MIMETypeList.Add(".xlw","application/vnd.ms-excel");
			MIMETypeList.Add(".xml","text/xml");
			MIMETypeList.Add(".xpl","audio/scpls");
			MIMETypeList.Add(".xsl","text/xml");
			MIMETypeList.Add(".z","application/x-compress");
			MIMETypeList.Add(".zip","application/x-zip-compressed");
		}

	    /// <summary>Returns the MIME content-type for the supplied file extension</summary>
		/// <returns>string MIME type (Example: \"text/plain\")</returns>
		public static string GetMimeType(string strFileName)
		{			
			try
			{
				string strFileExtension=new FileInfo(strFileName).Extension;
				string strContentType;
			    bool MONO = Environment.OSVersion.Platform == PlatformID.Unix;

				if(MONO)
				{
					strContentType=ContentType(strFileExtension);
				}
				else
				{
					Microsoft.Win32.RegistryKey extKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(strFileExtension);
					strContentType = (string)extKey.GetValue("Content Type");
				}

				if (strContentType != null)
				{	
					return strContentType; 
				}
				
                return "application/octet-stream";
			}
			catch(Exception)
			{
			    return "application/octet-stream";
			}
		}

	}
}

