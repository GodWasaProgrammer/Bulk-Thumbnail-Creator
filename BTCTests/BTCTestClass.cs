using Bulk_Thumbnail_Creator;
using UMapx.Colorspace;
using Xunit;

namespace BTCTests
{
	public class BTCTestClass
	{
		[Fact]
		private static void TestColorItem()
		{

			ColorItem TestColorItemFunctionality = new ColorItem();

			TestColorItemFunctionality.SetByHSL(80F,0.3F,1F);

			float HueResult = TestColorItemFunctionality.Hue;
			float SaturationResult = TestColorItemFunctionality.Saturation;
			float LightnessResult = TestColorItemFunctionality.Luminance;
			
			HSL resultHSL = new HSL(HueResult, SaturationResult, LightnessResult);

			HSL TestHSL = new HSL(80F, 0.3F, 1F);

			Assert.Equal(TestHSL, resultHSL);
			
		}
		[Fact]
		private static void HSLTester()
		{
			HSL TestHSL = new HSL(80F, 0.3F, 1F);

			float TestHue = TestHSL.Hue;
			float TestSaturation = TestHSL.Saturation;
			float TestLightness = TestHSL.Lightness;

			HSL ExpectedHSL = new HSL(TestHue, TestSaturation, TestLightness);

			Assert.Equal(TestHSL, ExpectedHSL);
		}
		
	}
}