/*********************************************************************
 *
 * tnef: extract files from microsoft TNEF format
 *
 *       scans tnef file and extracts all attachments
 *       attachments are written to their original file-names if possible
 *
 *       you have to get a TNET file first. Use metamail -w to extract
 *       TNEF file from mail enevelope.
 *
 *********************************************************************
 *
 * (C)1998 Thomas Boll  tb@boll.ch
 *         BOLL Engineering AG, CH-5430 Wettingen, Switzerland
 *         http://www.boll.ch
 *
 * free use of this software is granted to everyone according to the
 * GNU GPL.
 *
 *********************************************************************
 *
 * USE OF THIS SOFTWARE IS ON YOUR OWN RISK. THERE IS NO WARRANTY WHATSOEVER
 *
 * BECAUSE THE PROGRAM IS LICENSED FREE OF CHARGE, THERE IS NO WARRANTY
 * FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW.  EXCEPT WHEN
 * OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR OTHER PARTIES
 * PROVIDE THE PROGRAM "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED
 * OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.  THE ENTIRE RISK AS
 * TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU.  SHOULD THE
 * PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING,
 * REPAIR OR CORRECTION.
 *
 * IN NO EVENT UNLESS REQUIRED BY APPLICABLE LAW OR AGREED TO IN WRITING
 * WILL ANY COPYRIGHT HOLDER, OR ANY OTHER PARTY WHO MAY MODIFY AND/OR
 * REDISTRIBUTE THE PROGRAM AS PERMITTED ABOVE, BE LIABLE TO YOU FOR DAMAGES,
 * INCLUDING ANY GENERAL, SPECIAL, INCIDENTAL OR CONSEQUENTIAL DAMAGES ARISING
 * OUT OF THE USE OR INABILITY TO USE THE PROGRAM (INCLUDING BUT NOT LIMITED
 * TO LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY
 * YOU OR THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER
 * PROGRAMS), EVEN IF SUCH HOLDER OR OTHER PARTY HAS BEEN ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGES.
 *
 **********************************************************************/

#include <stdio.h>
#include <string.h>
#ifndef hpux
#include <getopt.h>
#endif

#define TNEF_SIGNATURE   0x223e9f78
#define LVL_MESSAGE      0x01
#define LVL_ATTACHMENT   0x02

#define _STRING  0x00010000
#define _BYTE    0x00060000
#define _WORD    0x00070000
#define _DWORD   0x00080000

#define AVERSION      (_DWORD|0x9006)
#define AMCLASS       (_WORD|0x8008)
#define ASUBJECT      (_DWORD|0x8004)
#define AFILENAME     (_STRING|0x8010)

#define ATTACHDATA    (_BYTE|0x800f)


#define GETINT32(p)    (p[0]+(p[1]<<8)+(p[2]<<16)+(p[3]<<24))
#define GETINT16(p)    (p[0]+(p[1]<<8))

extern char *strdup();
extern char *tempnam();

static int Verbose = 0;
static FILE *fdin = stdin;
static char *prog;
static int SkipSignature = 0;
static int SearchSignature = 0;
static long int Offset = 0;

int geti32 (void) {
    unsigned char buf[4];

    if (fread (buf, 4, 1, fdin) != 1) {
        fprintf (stderr, "unexpected end of input\n");
        exit (1);
    }
    return GETINT32(buf);
}

int geti16 (void) {
    unsigned char buf[2];

    if (fread (buf, 2, 1, fdin) != 1) {
        fprintf (stderr, "unexpected end of input\n");
        exit (1);
    }
    return GETINT16(buf);
}

int geti8 (void) {
    unsigned char buf[1];

    if (fread (buf, 1, 1, fdin) != 1) {
        fprintf (stderr, "unexpected end of input\n");
        exit (1);
    }
    return (int)buf[0];
}

main (int argc, char *argv[]) {

    unsigned char buf[4];
    unsigned int d;
    int cnt;
    int i;

    prog = argv[0];

    while ((i=getopt (argc, argv, "f:vhsxo:")) != EOF) {

        switch (i) {
        case 'v': 
            Verbose=1;  
            break;
        case 'f':
            fdin = fopen (optarg, "r");
            if (fdin == NULL) {
                perror (optarg);
                exit (20);
            }
            break;
        case 's':   /* skip signature */
            SkipSignature++;
            break;
        case 'x':   /* scan for signature */
            SearchSignature = 1;
            break;
        case 'o':   /* Offset */
            Offset = atol(optarg);
            break;
        case 'h':
        case '?':
            fprintf (stderr, "Usage: %s [-vsx] [-o offset] [-f inputfile]\n", 
                     argv[0]);
            exit (1);
        }
    }

    if (Offset) fseek (fdin, Offset, 0);

    /* try to scan for the ignature if the file is wrapped into
     * anything else.
     */
    if (SearchSignature) {
        long int lpos;

        for (lpos=0; ; lpos++) {

            if (fseek (fdin, lpos, 0)) {
                fprintf (stderr, "No signature found\n");
                exit (19);
            }
            d = geti32();
            if (d == TNEF_SIGNATURE) {
                if (Verbose) fprintf (stderr, "Signature found at %ld\n", lpos);
                break;
            }
        }
        fseek (fdin, lpos, 0);  /* re-position to signature */
    }

    if (SkipSignature < 2) {
        d = geti32();
        if (SkipSignature < 1) {
            if (d != TNEF_SIGNATURE) {
                fprintf (stderr, "Seems not to be a TNEF file\n");
                exit (2);
            }
        }
    }

    d = geti16();
    if (Verbose) fprintf (stderr, "TNEF Key is: %x\n", d);
    for (;;) {

        if (fread (buf, 1, 1, fdin) == 0) break;
        d = (unsigned)buf[0];

        switch (d) {
        case LVL_MESSAGE:
            if (Verbose) 
                fprintf (stderr, "%ld: Decoding Message Attributes\n", ftell(fdin));
            decode_message();
            break;
        case LVL_ATTACHMENT:
            if (Verbose) fprintf (stderr, "Decoding Attachment\n");
            decode_attachment();
            break;
        default:
        /*    fprintf (stderr, "Coding Error in TNEF file\n"); */
        /*    exit (5); */
        }
    }
}

decode_attribute (unsigned int d) {
    char buf[4000];
    unsigned int len;
    unsigned int v;
    unsigned int i;

    len = geti32();   /* data length */

    switch (d&0xffff0000) {
    case _BYTE:
        if (Verbose) fprintf (stderr, "Attribute %04x =", d&0xffff);
        for (i=0; i < len; i+=1) {
            v = geti8();
            if (Verbose) {
                if (i< 10) fprintf (stderr, " %02x", v);
                else if (i==10) fprintf (stderr, "...");
            }
        }
        if (Verbose) fprintf (stderr, "\n");
        break;
    case _WORD:
        if (Verbose) fprintf (stderr, "Attribute %04x =", d&0xffff);
        for (i=0; i < len; i+=2) {
            v = geti16();
            if (Verbose) {
                if (i < 6) fprintf (stderr, " %04x", v);
                else if (i==6) fprintf (stderr, "...");
            }
        }
        if (Verbose) fprintf (stderr, "\n");
        break;
    case _DWORD:
        if (Verbose) fprintf (stderr, "Attribute %04x =", d&0xffff);
        for (i=0; i < len; i+=4) {
            v = geti32();
            if (Verbose) {
                if (i < 4) fprintf (stderr, " %08x", v);
                else if (i==4) fprintf (stderr, "...");
            }
        }
        if (Verbose) fprintf (stderr, "\n");
        break;
    case _STRING:
        fread (buf, len, 1, fdin);
        if (Verbose) fprintf (stderr, "Attribute %04x = '%s'\n", d&0xffff, buf);
        break;
    default:
        fread (buf, len, 1, fdin);
        if (Verbose) fprintf (stderr, "Attribute %08x\n", d);
    }
    geti16();     /* checksum */
}

decode_message (void) {  
    unsigned int d;

    d = geti32();

    decode_attribute(d);
}


decode_attachment (void) {  

    char buf[4000];
    unsigned int d;
    unsigned int len;
    size_t i, chunk;
    FILE *fp;
    static char *fname = NULL;
    

    d = geti32();
    switch (d) {
    case AFILENAME:
        len = geti32();
        fread (buf, len, 1, fdin);
        if (Verbose) fprintf (stderr, "File-Name: %s\n", buf);
        if (fname) free(fname);
        fname = strdup (buf);
        geti16();     /* checksum */ 
        break;
    case ATTACHDATA:

        if (fname == NULL) fname = tempnam (NULL, "tnef");
        len = geti32();
        if (Verbose) fprintf (stderr, "ATTACH-DATA: %d bytes\n", len);
        fprintf (stderr, "%s: WRITING %s\n", prog, fname);
        fp = fopen (fname, "w");
        if (fp == NULL) {
            perror (fname);
            exit (10);
        }
        for (i = 0; i < len; ) {
            chunk = len-i;
            if (chunk > sizeof(buf)) chunk = sizeof(buf);
            fread (buf,  chunk, 1, fdin); 
            fwrite (buf, chunk, 1, fp);
            i += chunk;
        }
        fclose (fp);
        free (fname); fname = NULL;
        geti16();     /* checksum */ 
        break;
  
    default:
        decode_attribute(d);
    }
}

