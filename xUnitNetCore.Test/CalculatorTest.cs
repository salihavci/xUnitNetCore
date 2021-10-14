using Moq;
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
        public Mock<ICalculatorService> myMoq { get; set; }

        public CalculatorTest()
        {
            myMoq = new Mock<ICalculatorService>();
            _calculator = new Calculator(myMoq.Object);
            //_calculator = null;
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
        [InlineData(2, 5, 7)]
        [InlineData(10, 2, 12)]
        public void Add(int a, int b, int expectedTotal)
        {
            myMoq.Setup(x => x.add(a, b)).Returns(expectedTotal); //DI'daki metoda girmeden direk taklit ederek test sonucunu döndürüyorum.
            var calculator = _calculator;
            var actualData = calculator.add(a, b);
            Assert.Equal(actualData, expectedTotal);
        }

        [Theory]
        [InlineData(7, 5, 12)]
        [InlineData(5, 2, 7)]
        //Doğru isimlendirme şekli
        public void Add_SimpleValues_ReturnTotalValue(int a, int b, int exceptedTotal)
        {
            myMoq.Setup(x => x.add(a, b)).Returns(exceptedTotal); //Taklit etme işlemi (DI -> Dependency Injection)
            var calculator = _calculator;
            var total = calculator.add(a, b);
            Assert.Equal(total, exceptedTotal);
            myMoq.Verify(x => x.add(a, b), Times.Once); //Method bir kere çalışsın demek

        }

        [Theory]
        [InlineData(0, 5, 0)]
        [InlineData(5, 0, 0)]
        public void Add_ZeroValues_ReturnZeroValue(int a, int b, int exceptedTotal)
        {
            myMoq.Setup(x => x.add(a, b)).Returns(exceptedTotal);
            var calculator = _calculator;
            var total = calculator.add(a, b);
            Assert.Equal(total, exceptedTotal);
        }

        [Theory]
        [InlineData(5, 2, 10)]
        [InlineData(50, 6, 300)]
        public void Multiple_SimpleValues_ReturnMultiplesValues(int a, int b, int exceptedTotal)
        {

            //It.IsAny<int>() bu komut int tipinde herhangi bir değer alabileceğini söylüyor.
            int actualMultip = 0;
            myMoq.Setup(x => x.multiple(It.IsAny<int>(), It.IsAny<int>())).Callback<int, int>((x, y) => actualMultip = x * y); //Taklit etme işlemi (DI -> Dependency Injection)

            _calculator.multiple(a, b);
            Assert.Equal(exceptedTotal, actualMultip);
            _calculator.multiple(5,20);
            Assert.Equal(100, actualMultip);
        }

        [Theory]
        [InlineData(0, 5)]
        public void Multiple_ZeroValues_ReturnException(int a, int b)
        {
            myMoq.Setup(x => x.multiple(a, b)).Throws(new Exception("a=0 olamaz")); //Taklit etme işlemi (DI -> Dependency Injection)
            Exception ex = Assert.Throws<Exception>(()=> _calculator.multiple(a,b));
            Assert.Equal("a=0 olamaz", ex.Message);
        }

    }
}
