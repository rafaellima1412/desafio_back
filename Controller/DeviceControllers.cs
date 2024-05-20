using desafio.Models;
using Microsoft.AspNetCore.Mvc;

namespace desafio.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class DeviceController : ControllerBase
    {
        private static List<Device> devices = new List<Device>();

        [HttpGet]
        public IEnumerable<Device> Get() => devices;

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var device = devices.FirstOrDefault(d => d.Identifier == id);
            return device is not null ? Ok(device) : NotFound();
        }

        [HttpPost]
        public IActionResult Create(Device device)
        {
            devices.Add(device);
            return CreatedAtAction(nameof(GetById), new { id = device.Identifier }, device);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, Device updatedDevice)
        {
            var device = devices.FirstOrDefault(d => d.Identifier == id);
            if (device is null) return NotFound();
            device.Description = updatedDevice.Description;
            device.Manufacturer = updatedDevice.Manufacturer;
            device.Url = updatedDevice.Url;
            device.Commands = updatedDevice.Commands;
            return Ok(device);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var device = devices.FirstOrDefault(d => d.Identifier == id);
            if (device is null) return NotFound();
            devices.Remove(device);
            return Ok();
        }
    }
}


