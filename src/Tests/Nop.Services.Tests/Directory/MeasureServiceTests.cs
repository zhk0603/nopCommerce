using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Directory
{
    [TestFixture]
    public class MeasureServiceTests : ServiceTest
    {
        private IRepository<MeasureDimension> _measureDimensionRepository;
        private IRepository<MeasureWeight> _measureWeightRepository;
        private MeasureSettings _measureSettings;
        private IEventPublisher _eventPublisher;
        private IMeasureService _measureService;

        private MeasureDimension inches, feet, meters, millimetres;
        private MeasureWeight ounce, lb, kg, grams;
        
        [SetUp]
        public new void SetUp()
        {
            var measureDimensions = TestHelper.GetMeasureDimensions(1, 0.08333333M, 0.0254M, 25.4M);

            inches = measureDimensions[0];
            feet = measureDimensions[1];
            meters = measureDimensions[2];
            millimetres = measureDimensions[3];

            var measureWeights = TestHelper.GetMeasureWeights(16M, 1, 0.45359237M, 453.59237M);

            ounce = measureWeights[0];
            lb = measureWeights[1];
            kg = measureWeights[2];
            grams = measureWeights[3];

            _measureDimensionRepository = MockRepository.GenerateMock<IRepository<MeasureDimension>>();
            _measureDimensionRepository.Expect(x => x.Table).Return(new List<MeasureDimension> { inches, feet, meters, millimetres }.AsQueryable());
            _measureDimensionRepository.Expect(x => x.GetById(inches.Id)).Return(inches);
            _measureDimensionRepository.Expect(x => x.GetById(feet.Id)).Return(feet);
            _measureDimensionRepository.Expect(x => x.GetById(meters.Id)).Return(meters);
            _measureDimensionRepository.Expect(x => x.GetById(millimetres.Id)).Return(millimetres);

            _measureWeightRepository = MockRepository.GenerateMock<IRepository<MeasureWeight>>();
            _measureWeightRepository.Expect(x => x.Table).Return(new List<MeasureWeight> { ounce, lb, kg, grams }.AsQueryable());
            _measureWeightRepository.Expect(x => x.GetById(ounce.Id)).Return(ounce);
            _measureWeightRepository.Expect(x => x.GetById(lb.Id)).Return(lb);
            _measureWeightRepository.Expect(x => x.GetById(kg.Id)).Return(kg);
            _measureWeightRepository.Expect(x => x.GetById(grams.Id)).Return(grams);

            var cacheManager = new NopNullCache();

            _measureSettings = new MeasureSettings
            {
                //inch(es)
                BaseDimensionId = inches.Id,
                //lb(s)
                BaseWeightId = lb.Id
            };

            _eventPublisher = MockRepository.GenerateMock<IEventPublisher>();
            _eventPublisher.Expect(x => x.Publish(Arg<object>.Is.Anything));

            _measureService = new MeasureService(cacheManager,
                _measureDimensionRepository,
                _measureWeightRepository,
                _measureSettings, _eventPublisher);
        }

        [Test]
        public void Can_convert_dimension()
        {
            //from meter(s) to feet
            _measureService.ConvertDimension(10, meters, feet).ShouldEqual(32.81);
            //from inch(es) to meter(s)
            _measureService.ConvertDimension(10, inches, meters).ShouldEqual(0.25);
            //from meter(s) to meter(s)
            _measureService.ConvertDimension(13.333M, meters, meters).ShouldEqual(13.33);
            //from meter(s) to millimeter(s)
            _measureService.ConvertDimension(10, meters, millimetres).ShouldEqual(10000);
            //from millimeter(s) to meter(s)
            _measureService.ConvertDimension(10000, millimetres, meters).ShouldEqual(10);
        }

        [Test]
        public void Can_convert_weight()
        {
            //from ounce(s) to lb(s)
            _measureService.ConvertWeight(11, ounce, lb).ShouldEqual(0.69);
            //from lb(s) to ounce(s)
            _measureService.ConvertWeight(11, lb, ounce).ShouldEqual(176);
            //from ounce(s) to  ounce(s)
            _measureService.ConvertWeight(13.333M, ounce, ounce).ShouldEqual(13.33);
            //from kg(s) to ounce(s)
            _measureService.ConvertWeight(11, kg, ounce).ShouldEqual(388.01);
            //from kg(s) to gram(s)
            _measureService.ConvertWeight(10, kg, grams).ShouldEqual(10000);
        }
    }
}
