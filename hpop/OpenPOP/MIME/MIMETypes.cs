using System;
using System.IO;
using System.Collections;

namespace OpenPOP.MIME
{
	public static class MIMETypes
	{
	    private const string MIMEType_MSTNEF="application/ms-tnef";

        public static bool IsMSTNEF(string strContentType)
		{
			if(!string.IsNullOrEmpty(strContentType))
				if(strContentType.ToLower() == MIMEType_MSTNEF.ToLower())
					return true;
				else
					return false;
			
			return false;
		}

	    private static string ContentType(string strExtension)
		{
			if(MIMETypeList.ContainsKey(strExtension))
				return MIMETypeList[strExtension].ToString();
			
			return null;
		}

	    private static Hashtable MIMETypeList { get; set; }

		static MIMETypes()
		{
            MIMETypeList = new Hashtable
                               {
                                   {".323", "text/h323"},
                                   {".3gp", "video/3gpp"},
                                   {".3gpp", "video/3gpp"},
                                   {".acp", "audio/x-mei-aac"},
                                   {".act", "text/xml"},
                                   {".actproj", "text/plain"},
                                   {".ade", "application/msaccess"},
                                   {".adp", "application/msaccess"},
                                   {".ai", "application/postscript"},
                                   {".aif", "audio/aiff"},
                                   {".aifc", "audio/aiff"},
                                   {".aiff", "audio/aiff"},
                                   {".asf", "video/x-ms-asf"},
                                   {".asm", "text/plain"},
                                   {".asx", "video/x-ms-asf"},
                                   {".au", "audio/basic"},
                                   {".avi", "video/avi"},
                                   {".bmp", "image/bmp"},
                                   {".bwp", "application/x-bwpreview"},
                                   {".c", "text/plain"},
                                   {".cat", "application/vnd.ms-pki.seccat"},
                                   {".cc", "text/plain"},
                                   {".cdf", "application/x-cdf"},
                                   {".cer", "application/x-x509-ca-cert"},
                                   {".cod", "text/plain"},
                                   {".cpp", "text/plain"},
                                   {".crl", "application/pkix-crl"},
                                   {".crt", "application/x-x509-ca-cert"},
                                   {".cs", "text/plain"},
                                   {".css", "text/css"},
                                   {".csv", "application/vnd.ms-excel"},
                                   {".cxx", "text/plain"},
                                   {".dbs", "text/plain"},
                                   {".def", "text/plain"},
                                   {".der", "application/x-x509-ca-cert"},
                                   {".dib", "image/bmp"},
                                   {".dif", "video/x-dv"},
                                   {".dll", "application/x-msdownload"},
                                   {".doc", "application/msword"},
                                   {".dot", "application/msword"},
                                   {".dsp", "text/plain"},
                                   {".dsw", "text/plain"},
                                   {".dv", "video/x-dv"},
                                   {".edn", "application/vnd.adobe.edn"},
                                   {".eml", "message/rfc822"},
                                   {".eps", "application/postscript"},
                                   {".etd", "application/x-ebx"},
                                   {".etp", "text/plain"},
                                   {".exe", "application/x-msdownload"},
                                   {".ext", "text/plain"},
                                   {".fdf", "application/vnd.fdf"},
                                   {".fif", "application/fractals"},
                                   {".fky", "text/plain"},
                                   {".gif", "image/gif"},
                                   {".gz", "application/x-gzip"},
                                   {".h", "text/plain"},
                                   {".hpp", "text/plain"},
                                   {".hqx", "application/mac-binhex40"},
                                   {".hta", "application/hta"},
                                   {".htc", "text/x-component"},
                                   {".htm", "text/html"},
                                   {".html", "text/html"},
                                   {".htt", "text/webviewhtml"},
                                   {".hxx", "text/plain"},
                                   {".i", "text/plain"},
                                   {".iad", "application/x-iad"},
                                   {".ico", "image/x-icon"},
                                   {".ics", "text/calendar"},
                                   {".idl", "text/plain"},
                                   {".iii", "application/x-iphone"},
                                   {".inc", "text/plain"},
                                   {".infopathxml", "application/ms-infopath.xml"},
                                   {".inl", "text/plain"},
                                   {".ins", "application/x-internet-signup"},
                                   {".iqy", "text/x-ms-iqy"},
                                   {".isp", "application/x-internet-signup"},
                                   {".java", "text/java"},
                                   {".jfif", "image/jpeg"},
                                   {".jnlp", "application/x-java-jnlp-file"},
                                   {".jpe", "image/jpeg"},
                                   {".jpeg", "image/jpeg"},
                                   {".jpg", "image/jpeg"},
                                   {".jsl", "text/plain"},
                                   {".kci", "text/plain"},
                                   {".la1", "audio/x-liquid-file"},
                                   {".lar", "application/x-laplayer-reg"},
                                   {".latex", "application/x-latex"},
                                   {".lavs", "audio/x-liquid-secure"},
                                   {".lgn", "text/plain"},
                                   {".lmsff", "audio/x-la-lms"},
                                   {".lqt", "audio/x-la-lqt"},
                                   {".lst", "text/plain"},
                                   {".m1v", "video/mpeg"},
                                   {".m3u", "audio/mpegurl"},
                                   {".m4e", "video/mpeg4"},
                                   {".MAC", "image/x-macpaint"},
                                   {".mak", "text/plain"},
                                   {".man", "application/x-troff-man"},
                                   {".map", "text/plain"},
                                   {".mda", "application/msaccess"},
                                   {".mdb", "application/msaccess"},
                                   {".mde", "application/msaccess"},
                                   {".mdi", "image/vnd.ms-modi"},
                                   {".mfp", "application/x-shockwave-flash"},
                                   {".mht", "message/rfc822"},
                                   {".mhtml", "message/rfc822"},
                                   {".mid", "audio/mid"},
                                   {".midi", "audio/mid"},
                                   {".mk", "text/plain"},
                                   {".mnd", "audio/x-musicnet-download"},
                                   {".mns", "audio/x-musicnet-stream"},
                                   {".MP1", "audio/mp1"},
                                   {".mp2", "video/mpeg"},
                                   {".mp2v", "video/mpeg"},
                                   {".mp3", "audio/mpeg"},
                                   {".mp4", "video/mp4"},
                                   {".mpa", "video/mpeg"},
                                   {".mpe", "video/mpeg"},
                                   {".mpeg", "video/mpeg"},
                                   {".mpf", "application/vnd.ms-mediapackage"},
                                   {".mpg", "video/mpeg"},
                                   {".mpg4", "video/mp4"},
                                   {".mpga", "audio/rn-mpeg"},
                                   {".mpv2", "video/mpeg"},
                                   {".NMW", "application/nmwb"},
                                   {".nws", "message/rfc822"},
                                   {".odc", "text/x-ms-odc"},
                                   {".odh", "text/plain"},
                                   {".odl", "text/plain"},
                                   {".p10", "application/pkcs10"},
                                   {".p12", "application/x-pkcs12"},
                                   {".p7b", "application/x-pkcs7-certificates"},
                                   {".p7c", "application/pkcs7-mime"},
                                   {".p7m", "application/pkcs7-mime"},
                                   {".p7r", "application/x-pkcs7-certreqresp"},
                                   {".p7s", "application/pkcs7-signature"},
                                   {".PCT", "image/pict"},
                                   {".pdf", "application/pdf"},
                                   {".pdx", "application/vnd.adobe.pdx"},
                                   {".pfx", "application/x-pkcs12"},
                                   {".pic", "image/pict"},
                                   {".PICT", "image/pict"},
                                   {".pko", "application/vnd.ms-pki.pko"},
                                   {".png", "image/png"},
                                   {".pnt", "image/x-macpaint"},
                                   {".pntg", "image/x-macpaint"},
                                   {".pot", "application/vnd.ms-powerpoint"},
                                   {".ppa", "application/vnd.ms-powerpoint"},
                                   {".pps", "application/vnd.ms-powerpoint"},
                                   {".ppt", "application/vnd.ms-powerpoint"},
                                   {".prc", "text/plain"},
                                   {".prf", "application/pics-rules"},
                                   {".ps", "application/postscript"},
                                   {".pub", "application/vnd.ms-publisher"},
                                   {".pwz", "application/vnd.ms-powerpoint"},
                                   {".qt", "video/quicktime"},
                                   {".qti", "image/x-quicktime"},
                                   {".qtif", "image/x-quicktime"},
                                   {".qtl", "application/x-quicktimeplayer"},
                                   {".qup", "application/x-quicktimeupdater"},
                                   {".r1m", "application/vnd.rn-recording"},
                                   {".r3t", "text/vnd.rn-realtext3d"},
                                   {".RA", "audio/vnd.rn-realaudio"},
                                   {".RAM", "audio/x-pn-realaudio"},
                                   {".rat", "application/rat-file"},
                                   {".rc", "text/plain"},
                                   {".rc2", "text/plain"},
                                   {".rct", "text/plain"},
                                   {".rec", "application/vnd.rn-recording"},
                                   {".rgs", "text/plain"},
                                   {".rjs", "application/vnd.rn-realsystem-rjs"},
                                   {".rjt", "application/vnd.rn-realsystem-rjt"},
                                   {".RM", "application/vnd.rn-realmedia"},
                                   {".rmf", "application/vnd.adobe.rmf"},
                                   {".rmi", "audio/mid"},
                                   {".RMJ", "application/vnd.rn-realsystem-rmj"},
                                   {".RMM", "audio/x-pn-realaudio"},
                                   {".rms", "application/vnd.rn-realmedia-secure"},
                                   {".rmvb", "application/vnd.rn-realmedia-vbr"},
                                   {".RMX", "application/vnd.rn-realsystem-rmx"},
                                   {".RNX", "application/vnd.rn-realplayer"},
                                   {".rp", "image/vnd.rn-realpix"},
                                   {".RPM", "audio/x-pn-realaudio-plugin"},
                                   {".rqy", "text/x-ms-rqy"},
                                   {".rsml", "application/vnd.rn-rsml"},
                                   {".rt", "text/vnd.rn-realtext"},
                                   {".rtf", "application/msword"},
                                   {".rul", "text/plain"},
                                   {".RV", "video/vnd.rn-realvideo"},
                                   {".s", "text/plain"},
                                   {".sc2", "application/schdpl32"},
                                   {".scd", "application/schdpl32"},
                                   {".sch", "application/schdpl32"},
                                   {".sct", "text/scriptlet"},
                                   {".sd2", "audio/x-sd2"},
                                   {".sdp", "application/sdp"},
                                   {".sit", "application/x-stuffit"},
                                   {".slk", "application/vnd.ms-excel"},
                                   {".sln", "application/octet-stream"},
                                   {".SMI", "application/smil"},
                                   {".smil", "application/smil"},
                                   {".snd", "audio/basic"},
                                   {".snp", "application/msaccess"},
                                   {".spc", "application/x-pkcs7-certificates"},
                                   {".spl", "application/futuresplash"},
                                   {".sql", "text/plain"},
                                   {".srf", "text/plain"},
                                   {".ssm", "application/streamingmedia"},
                                   {".sst", "application/vnd.ms-pki.certstore"},
                                   {".stl", "application/vnd.ms-pki.stl"},
                                   {".swf", "application/x-shockwave-flash"},
                                   {".tab", "text/plain"},
                                   {".tar", "application/x-tar"},
                                   {".tdl", "text/xml"},
                                   {".tgz", "application/x-compressed"},
                                   {".tif", "image/tiff"},
                                   {".tiff", "image/tiff"},
                                   {".tlh", "text/plain"},
                                   {".tli", "text/plain"},
                                   {".torrent", "application/x-bittorrent"},
                                   {".trg", "text/plain"},
                                   {".txt", "text/plain"},
                                   {".udf", "text/plain"},
                                   {".udt", "text/plain"},
                                   {".uls", "text/iuls"},
                                   {".user", "text/plain"},
                                   {".usr", "text/plain"},
                                   {".vb", "text/plain"},
                                   {".vcf", "text/x-vcard"},
                                   {".vcproj", "text/plain"},
                                   {".viw", "text/plain"},
                                   {".vpg", "application/x-vpeg005"},
                                   {".vspscc", "text/plain"},
                                   {".vsscc", "text/plain"},
                                   {".vssscc", "text/plain"},
                                   {".wav", "audio/wav"},
                                   {".wax", "audio/x-ms-wax"},
                                   {".wbk", "application/msword"},
                                   {".wiz", "application/msword"},
                                   {".wm", "video/x-ms-wm"},
                                   {".wma", "audio/x-ms-wma"},
                                   {".wmd", "application/x-ms-wmd"},
                                   {".wmv", "video/x-ms-wmv"},
                                   {".wmx", "video/x-ms-wmx"},
                                   {".wmz", "application/x-ms-wmz"},
                                   {".wpl", "application/vnd.ms-wpl"},
                                   {".wprj", "application/webzip"},
                                   {".wsc", "text/scriptlet"},
                                   {".wvx", "video/x-ms-wvx"},
                                   {".XBM", "image/x-xbitmap"},
                                   {".xdp", "application/vnd.adobe.xdp+xml"},
                                   {".xfd", "application/vnd.adobe.xfd+xml"},
                                   {".xfdf", "application/vnd.adobe.xfdf"},
                                   {".xla", "application/vnd.ms-excel"},
                                   {".xlb", "application/vnd.ms-excel"},
                                   {".xlc", "application/vnd.ms-excel"},
                                   {".xld", "application/vnd.ms-excel"},
                                   {".xlk", "application/vnd.ms-excel"},
                                   {".xll", "application/vnd.ms-excel"},
                                   {".xlm", "application/vnd.ms-excel"},
                                   {".xls", "application/vnd.ms-excel"},
                                   {".xlt", "application/vnd.ms-excel"},
                                   {".xlv", "application/vnd.ms-excel"},
                                   {".xlw", "application/vnd.ms-excel"},
                                   {".xml", "text/xml"},
                                   {".xpl", "audio/scpls"},
                                   {".xsl", "text/xml"},
                                   {".z", "application/x-compress"},
                                   {".zip", "application/x-zip-compressed"}
                               };
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