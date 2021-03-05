using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Web_API_POC.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Device : ControllerBase
    {

        private readonly ILogger<Device> _logger;
        private readonly IConfiguration _configuration;

        public Device(ILogger<Device> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

        }
        /// <summary>
        /// Add new Device 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        /// <returns>New Created Todo Item</returns>
        /// <response code="200">Successfuly created new object</response>
        /// <response code="500">If the body is null</response>
        [HttpPost(Name = "DeviceCreated")]
        
        [ProducesResponseType(typeof(ErrorMessages), 500)]
        public IActionResult LogNewDevice([FromBody] Devices device)
        {
            string conn = _configuration["DBConnection"];
            String DeviceInsertion = "INSERT INTO medical_devicesss(DeviceName,DeviceType)";
            String DeviceInsertionVal = "VALUES(@DeviceName,@DeviceType)";
            try
            {
                if (device == null)
                {
                    _logger.LogError("Device is passed empty");
                    return BadRequest("Device is empty");
                }
                string deviceName = device.DeviceName;
                String deviceType = device.DeviceType;

                List<SqlParameter> deviceparameters = new List<SqlParameter>();
                deviceparameters.Add(new SqlParameter("@DeviceName", deviceName));
                deviceparameters.Add(new SqlParameter("@DeviceType", deviceType));
                String sqlStatement = DeviceInsertion + DeviceInsertionVal;
                int rowupdate = DBHelperpoc.DBhelper.ExecuteQueryNonWithParametersNoReturn(conn, sqlStatement, deviceparameters);
                if (rowupdate > 0)
                {
                    return CreatedAtRoute("DeviceCreated", $"{deviceName} is added to database");
                }
                else
                {
                    return StatusCode(500, " Internal Server Error");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"something error:{ex.Message}");
                return StatusCode(500, " Internal Server Error");
            }
        }

        /// <summary>
        /// Get All the device 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ErrorMessages), 500)]
        public IActionResult GetAllDevice()
        {
            try
            {
                // DB connection and Query
                string conn = _configuration["DBConnection"];
                string sqlSelectDevice = "SELECT *FROM MEDICAL_DEVICES";
                String JSONresult;

                DataTable dt = new DataTable();                
                dt = DBHelperpoc.DBhelper.ExecuteQueryWithParametersToDataTable(conn, sqlSelectDevice, null);

                JSONresult = JsonConvert.SerializeObject(dt);
                return StatusCode(200, JSONresult);
            }
            catch(Exception ex)
            {
                _logger.LogError($"something error:{ex.Message}");
                return StatusCode(500, " Internal Server Error");

            }

        }
    }
}
