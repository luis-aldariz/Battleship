using Battleship.Model;
using NUnit.Framework;
using System.Collections.Generic;

namespace Battleship.Tests
{
    [TestFixture]
    public class BattleshipServiceTests
    {
        private BattleshipService _battleshipService;
        private Player _player;

        [SetUp]
        public void SetUp()
        {
            _battleshipService = new BattleshipService();
            _player = new Player{ Id = 1, HitsGiven = new List<Point>{ new Point{ Row = 0,  Column = 0} } , SuccessfulShotsReceived = 0 };
        }

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void ValidateLocation_LocationEmpty_ReturnFalse(string location)
        {
            var result = _battleshipService.ValidateLocation(location);
            Assert.That(result, Is.False);
        }


        [Test]
        [TestCase("A1")]
        [TestCase("A1 A1 A1")]        
        [TestCase("11 11")]
        [TestCase("-")]
        [TestCase("- -")]      
        public void ValidateLocation_LocationInvalid_ReturnFalse(string location)
        {
            var result = _battleshipService.ValidateLocation(location);
            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("A0 C0")]
        [TestCase("A9 AC9")]
        [TestCase("I1 I3")]
        [TestCase("A1 A4")]
        [TestCase("A1 A8")]
        [TestCase("A1 D1")]
        [TestCase("A1 C3")]
        public void ValidateLocation_OutOfRangeLocation_ReturnFalse(string location)
        {
            var result = _battleshipService.ValidateLocation(location);
            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("A1 C1")]
        [TestCase("A1 A3")]
        [TestCase("a1 a3")]
        [TestCase("F8 H8")]
        [TestCase("H6 H8")]
        public void ValidateLocation_ValidRequest_ReturnTrue(string location)
        {
            var result = _battleshipService.ValidateLocation(location);
            Assert.That(result, Is.True);
        }



        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void ValidateShootLocation_ShootLocationEmpty_ReturnFalse(string point)
        {
            var result = _battleshipService.ValidateShootLocation(point, _player);
            Assert.That(result, Is.False);
        }


        [Test]
        [TestCase("A")]
        [TestCase("A11")]
        [TestCase("111")]
        [TestCase("-")]
        public void ValidateShootLocation_ShootLocationInvalid_ReturnFalse(string point)
        {
            var result = _battleshipService.ValidateShootLocation(point, _player);
            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("A0")]
        [TestCase("I1")]
        [TestCase("A9")]
        [TestCase("H9")]        
        public void ValidateShootLocation_OutOfRangeShootLocation_ReturnFalse(string point)
        {
            var result = _battleshipService.ValidateShootLocation(point, _player);
            Assert.That(result, Is.False);
        }

        [Test]        
        public void ValidateShootLocation_PointLocationAlreadySaved_ReturnFalse()
        {
            var point = "A1";
            var result = _battleshipService.ValidateShootLocation(point, _player);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ValidateShootLocation_ValidRequest_ReturnTrue()
        {
            var point = "B1";
            var result = _battleshipService.ValidateShootLocation(point, _player);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ValidateShipSink_LessThanThreeHits_ReturnFalse()
        {
            var result = _battleshipService.ValidateShipSink(_player);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ValidateShipSink_ThreeHits_ReturnTrue()
        {
            _player.SuccessfulShotsReceived = 3;
            var result = _battleshipService.ValidateShipSink(_player);
            Assert.That(result, Is.True);
        }
    }
}
