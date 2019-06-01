using Battleship.Model;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Battleship.Tests
{
    [TestFixture]
    public class BattleshipServiceTests
    {
        private BattleshipService _battleshipService;
        private Player _player1;
        private Player _player2;

        [SetUp]
        public void SetUp()
        {
            _battleshipService = new BattleshipService();
            _player1 = new Player{
                    Id = 1,
                    HitsGiven = new List<Point>(),
                    SuccessfulShotsReceived = 0,
                    Board = _battleshipService.CreateBoard()

                };
            _player2 = new Player{
                Id = 2,
                HitsGiven = new List<Point>(),
                SuccessfulShotsReceived = 0,
                Board = _battleshipService.CreateBoard()
            };
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
        public void ValidateLocation_LocationOutOfRange_ReturnFalse(string location)
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
        public void ValidateLocation_ValidLocation_ReturnTrue(string location)
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
            var result = _battleshipService.ValidateShootLocation(point, _player1);
            Assert.That(result, Is.False);
        }


        [Test]
        [TestCase("A")]
        [TestCase("A11")]
        [TestCase("111")]
        [TestCase("-")]
        public void ValidateShootLocation_ShootLocationInvalid_ReturnFalse(string point)
        {
            var result = _battleshipService.ValidateShootLocation(point, _player1);
            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("A0")]
        [TestCase("I1")]
        [TestCase("A9")]
        [TestCase("H9")]        
        public void ValidateShootLocation_ShootLocationOutOfRange_ReturnFalse(string point)
        {
            var result = _battleshipService.ValidateShootLocation(point, _player1);
            Assert.That(result, Is.False);
        }

        [Test]        
        public void ValidateShootLocation_ShootLocationAlreadySaved_ReturnFalse()
        {
            var point = "A1";
            _player1.HitsGiven.Add(new Point{ Column = 0, Row = 0});
            var result = _battleshipService.ValidateShootLocation(point, _player1);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ValidateShootLocation_ValidLocation_ReturnTrue()
        {
            var point = "B1";
            var result = _battleshipService.ValidateShootLocation(point, _player1);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ValidateShipSink_LessThanThreeHits_ReturnFalse()
        {
            var result = _battleshipService.ValidateShipSink(_player1);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ValidateShipSink_ThreeHits_ReturnTrue()
        {
            _player1.SuccessfulShotsReceived = 3;
            var result = _battleshipService.ValidateShipSink(_player1);
            Assert.That(result, Is.True);
        }

        [Test]
        public void AddShipPosition_SetPositionVertical_CreateCorrectnShipPoisition()
        {
            string location = "A1 A3";
             _battleshipService.AddShipPosition(location, _player1);
            Assert.That(_player1.ShipPosition.Count, Is.EqualTo(3));
            Assert.That(_player1.Board[0, 0], Is.EqualTo('S'));
            Assert.That(_player1.Board[1, 0], Is.EqualTo('S'));
            Assert.That(_player1.Board[2, 0], Is.EqualTo('S'));
        }

        [Test]
        public void AddShipPosition_SetPositionHorizontal_CreateCorrectnShipPoisition()
        {
            string location = "A1 C1";
            _battleshipService.AddShipPosition(location, _player1);
            Assert.That(_player1.ShipPosition.Count, Is.EqualTo(3));
            Assert.That(_player1.Board[0, 0], Is.EqualTo('S'));
            Assert.That(_player1.Board[0, 1], Is.EqualTo('S'));
            Assert.That(_player1.Board[0, 2], Is.EqualTo('S'));
        }

        [Test]
        public void SetShoot_SetNewShoot_CreateNewShoot()
        {
            string point = "A1";
            _player2.ShipPosition.Add(new Point{Column = 1, Row = 1});
            _battleshipService.SetShoot(point, _player1, _player2);

            Assert.That(_player1.HitsGiven.Count, Is.EqualTo(1));   
            Assert.That(_player1.HitsGiven.First().Column, Is.EqualTo(0));
            Assert.That(_player1.HitsGiven.First().Row, Is.EqualTo(0));

            Assert.That(_player2.SuccessfulShotsReceived, Is.EqualTo(0));
            Assert.That(_player2.Board[0,0], Is.EqualTo('X'));
        }

        [Test]
        public void SetShoot_SetSuccessfulShoot_CreateSuccessfulhoot()
        {
            string point = "A1";
            _player2.ShipPosition.Add(new Point { Column = 0, Row = 0 });
            _battleshipService.SetShoot(point, _player1, _player2);

            Assert.That(_player1.HitsGiven.Count, Is.EqualTo(1));
            Assert.That(_player1.HitsGiven.First().Column, Is.EqualTo(0));
            Assert.That(_player1.HitsGiven.First().Row, Is.EqualTo(0));

            Assert.That(_player2.SuccessfulShotsReceived, Is.EqualTo(1));
        }
    }
}
