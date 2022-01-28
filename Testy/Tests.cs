using Autofac.Extras.Moq;
using EquipmentRentalService.Models;
using EquipmentRentalService.Services;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace Tests
{
    public class Tests
    {
        [Fact]
        public void Test1()
        {

            using (var mock = AutoMock.GetLoose())
            {
                List<RentalEquipment> expected = GetAll();

                mock.Mock<IRentalService>().Setup(x => x.GetAll()).Returns(GetAll());

                var cls = mock.Create<IRentalService>();

                var actual = cls.GetAll();


                Assert.Equal(expected.Count, actual.Count);
            }
        }
        [Fact]
        public void Test2()
        {

            using (var mock = AutoMock.GetLoose())
            {



                RentalEquipment expected = GetItem(1);

                mock.Mock<IRentalService>().Setup(x => x.GetItem(1)).Returns(GetItem(1));

                var cls = mock.Create<IRentalService>();

                var actual = cls.GetItem(1);


                Assert.Equal(expected, actual);
            }
        }
        [Fact]
        public void Test3()
        {

            using (var mock = AutoMock.GetLoose())
            {



                RentalEquipment expected = new RentalEquipment
                {
                    Name = "Koparka CAT",
                    BaseRentPrice = 250.00,
                    DailyRentPrice = 75.00,
                    OverdueRate = 15,
                    IsAvailable = false
                };

                mock.Mock<IRentalService>().Setup(x => x.AddEquipment(expected));

                var cls = mock.Create<IRentalService>();

                cls.AddEquipment(expected);

            }
        }
        private List<RentalEquipment> GetAll()
        {
            List<RentalEquipment> list = new List<RentalEquipment>
            {
                new RentalEquipment
                {
                     Name = "Koparka CAT",
                    BaseRentPrice = 250.00,
                    DailyRentPrice = 75.00,
                    OverdueRate = 15,
                    IsAvailable = false
                },
                  new RentalEquipment
                {
                    Name = "Koparka CAT2",

                    BaseRentPrice = 250.00,
                    DailyRentPrice = 75.00,
                    OverdueRate = 15
                },

           new RentalEquipment
            {
                Name = "Koparka CAT3",

                BaseRentPrice = 250.00,
                DailyRentPrice = 75.00,
                OverdueRate = 15
            }

        };
            return list;
        }

        private RentalEquipment GetItem(int id)
        {
            List<RentalEquipment> list = GetAll();
            foreach (var item in list)
            {
                if (item.ID == id)
                {
                    return item;
                }
            }
            return null;
        }

    }
}