﻿/*
****************************************************************************************************
* IBScanUltimateApi_err.cs
*
* DESCRIPTION:
*     Error codes for IBScanUltimate.
*     http://www.integratedbiometrics.com
*
* NOTES:
*     Copyright (c) Integrated Biometrics, 2009-2017
*
* HISTORY:
*     2012/04/06  1.0.0  Created.
*     2012/09/05  1.3.0  Added error and warning codes (IBSU_ERR_DEVICE_ENABLED_POWER_SAVE_MODE, 
*                            IBSU_WRN_CHANNEL_IO_SLEEP_STATUS, IBSU_WRN_BGET_IMAGE)
*     2012/11/06  1.4.1  Added warning codes (IBSU_WRN_ROLLING_NOT_RUNNING)
*     2013/03/20  1.6.0  Added error and warning codes to support IBScanMatcher integration
*                            (IBSU_ERR_NBIS_NFIQ_FAILED, IBSU_WRN_EMPTY_IBSM_RESULT_IMAGE)
*     2013/08/03  1.6.9  Reformatted.
*     2013/10/14  1.7.0  Added error codes to check update firmware, invalid overlay handle
*                            (IBSU_ERR_DEVICE_NEED_UPDATE_FIRMWARE,IBSU_ERR_INVALID_OVERLAY_HANDLE )
*                        Added warning codes to deprecate API functions and detect no finger/
*                        incorrect fingers/smear in result image. 
*                            (IBSU_WRN_API_DEPRECATED, IBSU_WRN_NO_FINGER, IBSU_WRN_INCORRECT_FINGERS,
*                             IBSU_WRN_ROLLING_SMEAR)
*     2014/02/25  1.7.1  Added warning to check incorrect fingers/smear
*                            (IBSU_WRN_ROLLING_SHIFTED_HORIZONTALLY,IBSU_WRN_ROLLING_SHIFTED_VERTICALLY )
*     2014/07/23  1.8.0  Reformatted.
*                        Added error codes about WSQ
*                            (IBSU_ERR_NBIS_WSQ_ENCODE_FAILED,IBSU_ERR_NBIS_WSQ_DECODE_FAILED )
*     2014/09/17  1.8.1  Added error codes for JPEG2000 and PNG
*                            (IBSU_ERR_NBIS_PNG_ENCODE_FAILED,IBSU_ERR_NBIS_JP2_ENCODE_FAILED )
*     2015/04/07  1.8.4  Added error codes
*                            (IBSU_ERR_LIBRARY_UNLOAD_FAILED )
*                        Added warning codes
*                            (IBSU_WRN_ALREADY_ENHANCED_IMAGE )
*     2017/04/27  1.9.7  Added warning codes
*                            (IBSU_WRN_QUALITY_INVALID_AREA, IBSU_WRN_INVALID_BRIGHTNESS_FINGERS,
*                             IBSU_WRN_WET_FINGERS)
*     2017/06/16  1.9.8  Added error codes
*                            (IBSU_ERR_DEVICE_NEED_CALIBRATE_TOF, IBSU_WRN_MULTIPLE_FINGERS_DURING_ROLL)
*     2018/04/27  2.0.1  Added error codes
*                            (IBSU_ERR_DEVICE_INVALID_CALIBRATION_DATA, IBSU_ERR_DUPLICATE_EXTRACTION_FAILED,
*                             IBSU_ERR_DUPLICATE_ALREADY_USED, IBSU_ERR_DUPLICATE_SEGMENTATION_FAILED,
*                             IBSU_ERR_DUPLICATE_MATCHING_FAILED)
****************************************************************************************************
*/

using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace IBscanUltimate
{
    public partial class DLL
    {
        /*
        ****************************************************************************************************
        * GENERIC ERROR CODES
        ****************************************************************************************************
        */
        public const int IBSU_STATUS_OK                             = 0;  /* Function completed successfully. */
        public const int IBSU_ERR_INVALID_PARAM_VALUE               = -1;  /* Invalid parameter value. */
        public const int IBSU_ERR_MEM_ALLOC                         = -2;  /* Insufficient memory. */
        public const int IBSU_ERR_NOT_SUPPORTED                     = -3;  /* Requested functionality isn't supported. */
        public const int IBSU_ERR_FILE_OPEN                         = -4;  /* File (USB handle, pipe, or image file) open failed. */
        public const int IBSU_ERR_FILE_READ                         = -5;  /* File (USB handle, pipe, or image file) read failed. */
        public const int IBSU_ERR_RESOURCE_LOCKED                   = -6;  /* Failure due to a locked resource. */
        public const int IBSU_ERR_MISSING_RESOURCE                  = -7;  /* Failure due to a missing resource (e.g. DLL file). */
        public const int IBSU_ERR_INVALID_ACCESS_POINTER            = -8;  /* Invalid access pointer address. */
        public const int IBSU_ERR_THREAD_CREATE                     = -9;  /* Thread creation failed. */
        public const int IBSU_ERR_COMMAND_FAILED                    = -10;  /* Generic command execution failed. */
        public const int IBSU_ERR_LIBRARY_UNLOAD_FAILED             = -11;  /* The library unload failed. */
        
        /*
        ****************************************************************************************************
        * LOW-LEVEL I/O ERROR CODES
        ****************************************************************************************************
        */
        public const int IBSU_ERR_CHANNEL_IO_COMMAND_FAILED         = -100;  /* Command execution failed. */
        public const int IBSU_ERR_CHANNEL_IO_READ_FAILED            = -101;  /* Input communication failed. */
        public const int IBSU_ERR_CHANNEL_IO_WRITE_FAILED           = -102;  /* Output communication failed. */
        public const int IBSU_ERR_CHANNEL_IO_READ_TIMEOUT           = -103;  /* Input command execution timed out, but device communication is alive. */
        public const int IBSU_ERR_CHANNEL_IO_WRITE_TIMEOUT          = -104;  /* Output command execution timed out, but device communication is alive. */
        public const int IBSU_ERR_CHANNEL_IO_UNEXPECTED_FAILED      = -105;  /* Unexpected communication failed. (Only used on IBTraceLogger.) */
        public const int IBSU_ERR_CHANNEL_IO_INVALID_HANDLE         = -106;  /* I/O handle state is invalid; reinitialization (close then open) required. */
        public const int IBSU_ERR_CHANNEL_IO_WRONG_PIPE_INDEX       = -107;  /* I/O pipe index is invalid; reinitialization (close then open) required. */

        /*
        ****************************************************************************************************
        * DEVICE-RELATED ERROR CODES
        ****************************************************************************************************
        */
        public const int IBSU_ERR_DEVICE_IO                         = -200;  /* Device communication failed. */
        public const int IBSU_ERR_DEVICE_NOT_FOUND                  = -201;  /* No device is detected/active. */
        public const int IBSU_ERR_DEVICE_NOT_MATCHED                = -202;  /* No matching device is detected. */
        public const int IBSU_ERR_DEVICE_ACTIVE                     = -203;  /* Initialization failed because it is in use by another thread/process. */
        public const int IBSU_ERR_DEVICE_NOT_INITIALIZED            = -204;  /* Device needs to be initialized. */
        public const int IBSU_ERR_DEVICE_INVALID_STATE              = -205;  /* Device state is invalid; reinitialization (exit then initialization) required. */
        public const int IBSU_ERR_DEVICE_BUSY                       = -206;  /* Another thread is currently using device functions. */
        public const int IBSU_ERR_DEVICE_NOT_SUPPORTED_FEATURE      = -207;  /* No hardware support for requested function. */
        public const int IBSU_ERR_INVALID_LICENSE                   = -208;  /* The license is invalid or does not match to the device. */
        public const int IBSU_ERR_USB20_REQUIRED                    = -209;  /* Device is connected to a full-speed USB port but high-speed is required. */
        public const int IBSU_ERR_DEVICE_ENABLED_POWER_SAVE_MODE    = -210;  /* Device is enabled the power save mode. */
        public const int IBSU_ERR_DEVICE_NEED_UPDATE_FIRMWARE       = -211;  /* Need to update firmware. */
		public const int IBSU_ERR_DEVICE_NEED_CALIBRATE_TOF         = -212;  /* Need to calibrate TOF. */
		public const int IBSU_ERR_DEVICE_INVALID_CALIBRATION_DATA   = -213;  /* Invalid calibration data from the device. */

        /*
        ****************************************************************************************************
        * IMAGE CAPTURE-RELATED ERROR CODES
        ****************************************************************************************************
        */
        public const int IBSU_ERR_CAPTURE_COMMAND_FAILED            = -300;  /* Image acquisition failed. */  
        public const int IBSU_ERR_CAPTURE_STOP                      = -301;  /* Stop capture failed. */
        public const int IBSU_ERR_CAPTURE_TIMEOUT                   = -302;  /* Timeout during capturing. */
        public const int IBSU_ERR_CAPTURE_STILL_RUNNING             = -303;  /* A capture is still running. */
        public const int IBSU_ERR_CAPTURE_NOT_RUNNING               = -304;  /* A capture is not running. */
        public const int IBSU_ERR_CAPTURE_INVALID_MODE              = -305;  /* Capture mode is not valid or not supported. */
        public const int IBSU_ERR_CAPTURE_ALGORITHM                 = -306;  /* Generic algorithm processing failure. */
        public const int IBSU_ERR_CAPTURE_ROLLING                   = -307;  /* Image processing failure at rolled finger print processing. */
        public const int IBSU_ERR_CAPTURE_ROLLING_TIMEOUT           = -308;  /* No roll start detected within a defined timeout period. */

        /*
        ****************************************************************************************************
        * CLIENT WINDOW-RELATED ERROR CODES
        ****************************************************************************************************
        */
        public const int IBSU_ERR_CLIENT_WINDOW                     = -400;  /* Generic client window failure. */
        public const int IBSU_ERR_CLIENT_WINDOW_NOT_CREATE          = -401;  /* No client window has been created. */
        public const int IBSU_ERR_INVALID_OVERLAY_HANDLE            = -402;  /* Invalid overlay handle. */

        /*
        ****************************************************************************************************
        * NBIS-RELATED ERROR CODES
        ****************************************************************************************************
        */
        public const int IBSU_ERR_NBIS_NFIQ_FAILED                  = -500;  /* Getting NFIQ score failed. */
        public const int IBSU_ERR_NBIS_WSQ_ENCODE_FAILED            = -501;  /* WSQ encode failed. */
        public const int IBSU_ERR_NBIS_WSQ_DECODE_FAILED            = -502;  /* WSQ decode failed. */
        public const int IBSU_ERR_NBIS_PNG_ENCODE_FAILED            = -503;  /* PNG decode failed. */
        public const int IBSU_ERR_NBIS_JP2_ENCODE_FAILED            = -504;  /* JP2 decode failed. */

		/*
		****************************************************************************************************
        * DUPLICATE-RELATED ERROR CODES
		****************************************************************************************************
		*/
        public const int IBSU_ERR_DUPLICATE_EXTRACTION_FAILED       = -600;    /* When the extraction from the fingerimage is faild in IBSU_ADDFingerImage and DLL_IsFingerDuplicated */
        public const int IBSU_ERR_DUPLICATE_ALREADY_USED            = -601;    /* When the image of the fingerposition is already in use. in IBSU_ADDFingerImage */
        public const int IBSU_ERR_DUPLICATE_SEGMENTATION_FAILED     = -602;    /* When found segment fingercounts are not two and more in IBSU_IsValidFingerGeometry */
        public const int IBSU_ERR_DUPLICATE_MATCHING_FAILED     		= -603;    /* When found small extrations in IBSM_MatchingTemplate */
        
        /*
        ****************************************************************************************************
        * WARNING CODES
        ****************************************************************************************************
        */
        public const int IBSU_WRN_CHANNEL_IO_FRAME_MISSING          = 100;  /* Missing an image frame. (Only used on IBTraceLogger.) */
        public const int IBSU_WRN_CHANNEL_IO_CAMERA_WRONG           = 101;  /* Camera work is wrong. reset is requied (Only used on IBTraceLogger.) */
        public const int IBSU_WRN_CHANNEL_IO_SLEEP_STATUS           = 102;
        public const int IBSU_WRN_OUTDATED_FIRMWARE                 = 200;  /* Device firmware version outdated. */
        public const int IBSU_WRN_ALREADY_INITIALIZED               = 201;  /* Device/component has already been initialized and is ready to be used. */
        public const int IBSU_WRN_API_DEPRECATED                    = 202;  /* API function was deprecated. */
        public const int IBSU_WRN_ALREADY_ENHANCED_IMAGE            = 203;  /* Image has already been enhanced. */
        public const int IBSU_WRN_BGET_IMAGE                        = 300;  /* Device still has not gotten the first image frame. */
        public const int IBSU_WRN_ROLLING_NOT_RUNNING               = 301;  /* Rolling has not started. */
        public const int IBSU_WRN_NO_FINGER                         = 302;  /* No finger detected in result image. */
        public const int IBSU_WRN_INCORRECT_FINGERS                 = 303;  /* Incorrect fingers detected in result image. */
        public const int IBSU_WRN_ROLLING_SMEAR                     = 304;  /* Smear detected in rolled result image. */
        public const int IBSU_WRN_EMPTY_IBSM_RESULT_IMAGE           = 400;  /* Empty result image. */
        public const int IBSU_WRN_QUALITY_INVALID_AREA              = 512;  /* When a finger is located on the invalid area */
        public const int IBSU_WRN_INVALID_BRIGHTNESS_FINGERS        = 600;  /* When a finger doesn't meet image brightness criteria */
        public const int IBSU_WRN_WET_FINGERS                       = 601;  /* When detected wet finger */
        public const int IBSU_WRN_MULTIPLE_FINGERS_DURING_ROLL	    = 602;  /* When detected multiple fingers during roll */
        public const int IBSU_WRN_SPOOF_DETECTED                    = 603;  /* When detected spoof finger */
        

        /*
        ****************************************************************************************************
        * WARNING CODES FOR SMEAR
        * note this value is added to WRN_ROLLING_SMEAR code 304
        * 305 is smear shifted horizontally, 306 is shifted vertically, 307 is shifted both horizontally and vertically
        ****************************************************************************************************
        */
        public const int IBSU_WRN_ROLLING_SHIFTED_HORIZONTALLY      = (IBSU_WRN_ROLLING_SMEAR | 1);    /* Rolled finger was shifted horizontally. */
        public const int IBSU_WRN_ROLLING_SHIFTED_VERTICALLY        = (IBSU_WRN_ROLLING_SMEAR | 2);    /* Rolled finger was shifted vertically. */

        /*
        ****************************************************************************************************
        * WARNING CODES FOR INVALID AREA
        ****************************************************************************************************
        */
        public const int IBSU_WRN_QUALITY_INVALID_AREA_HORIZONTALLY		= (IBSU_WRN_QUALITY_INVALID_AREA | 1);  /* Finger was located on the horizontal invalid area */
        public const int IBSU_WRN_QUALITY_INVALID_AREA_VERTICALLY       = (IBSU_WRN_QUALITY_INVALID_AREA | 2);  /* Finger was located on the vertical invalid area */
    }
}
