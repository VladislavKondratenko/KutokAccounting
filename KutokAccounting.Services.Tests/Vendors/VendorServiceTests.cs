using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Vendors;
using KutokAccounting.Services.Vendors.Models;
using NSubstitute;

namespace KutokAccounting.Service.Tests.Vendors;

public class VendorServiceTests
{
	[Fact]
	public async Task CreateVendor()
	{
		//Arrange
		IVendorService? vendorService = Substitute.For<IVendorService>();

		VendorDto vendorDto = new()
		{
			Description = null,
			Name = new string('a', 1000)
		};

		//Act
		Vendor? result = await vendorService.CreateAsync(vendorDto, default);

		//Assert
		Assert.Null(result);
	}
}