using AutoFixture;
using Moq;
using Raven.NET.Core.Exceptions;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Providers;
using Raven.NET.Core.Providers.Interfaces;
using Raven.NET.Core.Storage;
using Raven.NET.Core.Storage.Interfaces;
using Raven.NET.Core.Tests.SubjectTests;
using Shouldly;
using Xunit;

namespace Raven.NET.Core.Tests.ProviderTests;

[Collection("Tests")]
public class RavenProviderTests
{
    private readonly IRavenProvider sut;
    private readonly Mock<IRavenWatcher> _ravenWatcher;
    private readonly Mock<IRavenTypeWatcher> _ravenTypeWatcher;
    
    private readonly Mock<IRavenStorage> _ravenStorage;
    
    public RavenProviderTests()
    {
        _ravenStorage = new Mock<IRavenStorage>();
        _ravenWatcher = new Mock<IRavenWatcher>();
        _ravenTypeWatcher = new Mock<IRavenTypeWatcher>();
        sut = new RavenProvider(_ravenStorage.Object);
    }

    [Fact]
    void AddRaven_Should_AddRavenToInternalRavenCache_WhenNoTypeProvided()
    {
        //Arrange 
        var raven = _ravenWatcher.Object;
        var name = "TestWatcher";
        _ravenStorage.Setup(x => x.RavenWatcherTryAdd(name, raven)).Returns(true);

        //Act
        sut.AddRaven(name, raven);
        
        //Assert
        _ravenStorage.Verify(x => x.RavenWatcherTryAdd(name, raven), Times.Once);
        //_ravenStorage.Object.RavenWatcherExists(name).ShouldBeTrue();
    }
    
    [Fact]
    void AddRaven_Should_ThrowException_WhenRavenAlreadyExists()
    {
        //Arrange 
        var raven = _ravenWatcher.Object;
        var name = "TestWatcher";
        _ravenStorage.Object.RavenWatcherTryAdd(name, raven);

        //Act
        var ex = Record.Exception(() => sut.AddRaven(name, raven));
        
        //Assert
        ex.ShouldNotBeNull();
        ex.ShouldBeOfType<RavenAlreadyExistsException>();
    }
    
    [Fact]
    void AddRaven_Should_AddRavenToInternalRavenTypeCache_WhenTypeProvided()
    {
        //Arrange 
        var raven = _ravenTypeWatcher.Object;
        var name = "TestWatcher";
        _ravenStorage.Setup(x => x.RavenTypeWatcherTryAdd(typeof(TestSubjectEntity), raven)).Returns(true);
        _ravenStorage.Setup(x => x.RavenWatcherTryAdd(name, raven)).Returns(true);

        //Act
        sut.AddRaven(name, raven, typeof(TestSubjectEntity));
        
        //Assert
        _ravenStorage.Verify(x => x.RavenTypeWatcherTryAdd(typeof(TestSubjectEntity), raven), Times.Once);
        _ravenStorage.Verify(x => x.RavenWatcherTryAdd(name, raven), Times.Once);
        //_ravenStorage.Object.RavenTypeWatcherExists(typeof(TestSubjectEntity)).ShouldBeTrue();
    }
    
    [Fact]
    void AddRaven_Should_ThrowException_WhenRavenTypeAlreadyExists()
    {
        //Arrange 
        var raven = _ravenTypeWatcher.Object;
        var name = "TestWatcher";
        _ravenStorage.Object.RavenTypeWatcherTryAdd(typeof(TestSubjectEntity), raven);

        //Act
        var ex = Record.Exception(() => sut.AddRaven(name, raven, typeof(TestSubjectEntity)));
        
        //Assert
        ex.ShouldNotBeNull();
        ex.ShouldBeOfType<RavenForTypeAlreadyExistsException>();
    }
    
    [Fact]
    void RemoveRaven_Should_RemoveRavenFromInternalCache()
    {
        //Arrange 
        var raven = _ravenWatcher.Object;
        var name = "TestWatcher";
        _ravenStorage.Object.RavenWatcherTryAdd(name, raven);
        _ravenStorage.Setup(x => x.RavenWatcherExists(name)).Returns(true);

        //Act
        sut.RemoveRaven(name);
        
        //Assert
        _ravenStorage.Verify(x => x.RavenWatcherRemove(name), Times.Once);
    }
    
    [Fact]
    void RemoveRaven_Should_ThrowExceptionWhenRavenDoesNotExist()
    {
        //Arrange 
        var raven = _ravenWatcher.Object;
        var name = "TestWatcher";

        //Act
        var ex = Record.Exception(() => sut.RemoveRaven(name));
        
        //Assert
        ex.ShouldNotBeNull();
        ex.ShouldBeOfType<RavenDoesNotExistsException>();
    }
    
    [Fact]
    void GetRaven_Should_ReturnRavenWatcher_WhenNoTypeProvided()
    {
        //Arrange 
        
        var raven = _ravenWatcher.Object;
        var name = "TestWatcher";
        _ravenStorage.Object.RavenWatcherTryAdd(name, raven);
        _ravenStorage.Setup(x => x.RavenWatcherExists(name)).Returns(true);
        _ravenStorage.Setup(x => x.RavenWatcherGet(name)).Returns(raven);
        
        //Act
        var result = sut.GetRaven(name);
        
        //Assert
        result.ShouldNotBeNull();
        result.ShouldBe(raven);
    }
    
    [Fact]
    void GetRaven_Should_ReturnException_WhenRavenDoesNotExist()
    {
        //Arrange 
        var raven = _ravenWatcher.Object;
        var name = "TestWatcher";
        
        //Act
        var ex = Record.Exception(() => sut.GetRaven(name));
        
        //Assert
        ex.ShouldNotBeNull();
        ex.ShouldBeOfType<RavenDoesNotExistsException>();
    }
    
    [Fact]
    void GetRaven_Should_ReturnRavenTypeWatcher_WhenTypeProvided()
    {
        //Arrange 
        var raven = _ravenTypeWatcher.Object;
        var name = "TestWatcher";
        _ravenStorage.Object.RavenTypeWatcherTryAdd(typeof(TestSubjectEntity), raven);
        _ravenStorage.Setup(x => x.RavenTypeWatcherExists(typeof(TestSubjectEntity))).Returns(true);
        _ravenStorage.Setup(x => x.RavenTypeWatcherGet(typeof(TestSubjectEntity))).Returns(raven);

        //Act
        var result = sut.GetRaven(name, typeof(TestSubjectEntity));
        
        //Assert
        result.ShouldNotBeNull();
    }
    
    [Fact]
    void GetRaven_Should_ReturnException_WhenTypeRavenDoesNotExist()
    {
        //Arrange 
        var raven = _ravenTypeWatcher.Object;
        var name = "TestWatcher";
        _ravenStorage.Setup(x => x.RavenWatcherExists(name)).Returns(false);
        
        //Act
        var ex = Record.Exception(() => sut.GetRaven(name, typeof(TestSubjectEntity)));
        
        //Assert
        ex.ShouldNotBeNull();
        ex.ShouldBeOfType<RavenDoesNotExistsException>();
    }
    
    [Fact]
    void UpdateRavens_Should_CallUpdateOnEveryWatcherRegistered()
    {
        //Arrange 
        var fixture = new Fixture();
        var raven = _ravenWatcher.Object;
        var name = "TestWatcher";
        _ravenStorage.Object.RavenWatcherTryAdd(name, raven);
        var mockedSubject = fixture.Create<TestSubjectEntity>();
        mockedSubject.Attach(raven);
        
        //Act
        sut.UpdateRavens(mockedSubject);
        
        //Assert
        _ravenWatcher.Verify(x => x.Update(mockedSubject));
    }
}