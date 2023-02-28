using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Thumbnail_Creator
{
	public class UI
	{
		public static int TakeNumberOfThumbNails()
		{
			bool isParsable;
			int inputAsInt;
			do
			{
				Console.WriteLine("Input the amount of Thumbnails you wish to create");

				string input = Console.ReadLine();
				isParsable = int.TryParse(input, out inputAsInt);

				if (!isParsable)
				{
					Console.WriteLine("Couldnt parse your input to an int. try again");
				}
				else if(isParsable)
				{
					Console.WriteLine("Successfully Parsed your input.");
				}

			} while (isParsable == false);

			return inputAsInt;
		}
	}
}
