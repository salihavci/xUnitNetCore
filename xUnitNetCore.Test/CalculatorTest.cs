using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using xUnitNetCore.App;

namespace xUnitNetCore.Test
{
    public class CalculatorTest
    {
        public Calculator _calculator { get; set; }

        public CalculatorTest()
        {
            _calculator = new Calculator();
        }

        [Fact] //Test etmek için yazılması zorunlu olan attribute (Parametre almazsa fonksiyon bu kullanılır)
        public void AddTest()
        {
            ////Arrange = Değişkenleri initalize ettiğimiz evre
            //int a = 5;
            //int b = 20;
            //var calc = new Calculator();
            ////Act = Test edilecek methodları çalıştıracağımız evre.
            //var total = calc.add(a, b);
            ////Asert = Doğrulama evresidir. Total gerçekten doğru mu geliyor ? 
            //Assert.Equal<int>(25, total);

            Assert.Contains("Salih", "Salih AVCI");
            Assert.DoesNotContain("itu", "cacabeystjr@gmail.com");
            var names = new List<string>() { "Salih", "Mert", "Çağrı", "Tayfun" };
            Assert.Contains(names, x => x == "Mert");
            Assert.True(5 > 3);
            Assert.False(5 != 5);
            Assert.True("".GetType() == typeof(string));
            Assert.Matches("^dog", "dogs");
            Assert.DoesNotMatch("^dog", "Elodog");
            Assert.StartsWith("Sa", "Salih Avcı");
            Assert.EndsWith("cı", "Salih Avcı");
            var deneme = new List<string>();
            Assert.Empty(deneme);
            Assert.NotEmpty(names);
            Assert.InRange<int>(2, 1, 10);
            Assert.NotInRange<int>(-1, 10, 100);
            deneme.Add("Test");
            Assert.Single<string>(deneme);
            Assert.IsType<List<string>>(names);
            Assert.IsNotType<List<int>>(deneme);
            Assert.IsAssignableFrom<IEnumerable<string>>(new List<string>());
            Assert.IsAssignableFrom<Object>("Salih");
            Assert.IsAssignableFrom<Object>(2);
            string deger = null;
            Assert.Null(deger);
            Assert.NotNull("Salih");

        }

        //[Theory] == Bu attribute parametre almak zorunda
        //[InlineDate(val1,val2,val3....)] == Parametre girişi için oluşturulan attribute

        [Theory]
        [InlineData(2,5,7)]
        [InlineData(10,2,12)]
        public void Add(int a, int b, int expectedTotal)
        {
            var calculator = _calculator;
            var actualData = calculator.add(a, b);
            Assert.Equal(actualData, expectedTotal);
        }
    }
}
