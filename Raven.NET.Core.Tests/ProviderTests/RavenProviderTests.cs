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
    
    private readonly IRavenStorage _ravenStorage;
    
    public RavenProviderTests()
    {
        _ravenWatcher = new Mock<IRavenWatcher>();
        _ravenTypeWatcher = new Mock<IRavenTypeWatcher>();
        _ravenStorage = RavenStorage.Instance;
        sut = new RavenProvider();
    }

    [Fact]
    void AddRaven_Should_AddRavenToInternalRavenCache_WhenNoTypeProvided()
    {
        //Arrange 
        var raven = _ravenWatcher.Object;
        var name = "TestWatcher";

        //Act
        sut.AddRaven(name, raven);
        
        //Assert
        _ravenStorage.RavenWatcherStorage.ShouldContainKey(name);
        
        _ravenStorage.RavenWatcherStorage.Clear();
    }
    
    [Fact]
    void AddRaven_Should_ThrowException_WhenRavenAlreadyExists()
    {
        //Arrange 
        var raven = _ravenWatcher.Object;
        var name = "TestWatcher";
        _ravenStorage.RavenWatcherStorage.TryAdd(name, raven);

        //Act
        var ex = Record.Exception(() => sut.AddRaven(name, raven));
        
        //Assert
        ex.ShouldNotBeNull();
        ex.ShouldBeOfType<RavenAlreadyExistsException>();
        
        _ravenStorage.RavenWatcherStorage.Clear();

    }
    
    [Fact]
    void AddRaven_Should_AddRavenToInternalRavenTypeCache_WhenTypeProvided()
    {
        //Arrange 
        _ravenStorage.RavenTypeWatcherStorage.Clear();
        var raven = _ravenTypeWatcher.Object;
        var name = "TestWatcher";

        //Act
        sut.AddRaven(name, raven, typeof(TestSubjectEntity));
        
        //Assert
        _ravenStorage.RavenTypeWatcherStorage.ShouldContainKey(typeof(TestSubjectEntity));
    }
    
    [Fact]
    void AddRaven_Should_ThrowException_WhenRavenTypeAlreadyExists()
    {
        //Arrange 
        var raven = _ravenTypeWatcher.Object;
        var name = "TestWatcher";
        _ravenStorage.RavenTypeWatcherStorage.TryAdd(typeof(TestSubjectEntity), raven);

        //Act
        var ex = Record.Exception(() => sut.AddRaven(name, raven, typeof(TestSubjectEntity)));
        
        //Assert
        ex.ShouldNotBeNull();
        ex.ShouldBeOfType<RavenForTypeAlreadyExistsException>();
        
        _ravenStorage.RavenTypeWatcherStorage.Clear();
    }
    
    [Fact]
    void RemoveRaven_Should_RemoveRavenFromInternalCache()
    {
        //Arrange 
        var raven = _ravenWatcher.Object;
        var name = "TestWatcher";
        _ravenStorage.RavenWatcherStorage.TryAdd(name, raven);

        //Act
        sut.RemoveRaven(name);
        
        //Assert
        _ravenStorage.RavenWatcherStorage.ShouldBeEmpty();
    }
    
    [Fact]
    void RemoveRaven_Should_ThrowExceptionWhenRavenDoesNotExist()
    {
        //Arrange 
        _ravenStorage.RavenWatcherStorage.Clear();
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
        _ravenStorage.RavenWatcherStorage.TryAdd(name, raven);
        
        //Act
        var result = sut.GetRaven(name);
        
        //Assert
        result.ShouldNotBeNull();
        result.ShouldBe(raven);
        
        _ravenStorage.RavenWatcherStorage.Clear();
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
        _ravenStorage.RavenTypeWatcherStorage.TryAdd(typeof(TestSubjectEntity), raven);
        
        //Act
        var result = sut.GetRaven(name, typeof(TestSubjectEntity));
        
        //Assert
        result.ShouldNotBeNull();
        
        _ravenStorage.RavenTypeWatcherStorage.Clear();
    }
    
    [Fact]
    void GetRaven_Should_ReturnException_WhenTypeRavenDoesNotExist()
    {
        //Arrange 
        _ravenStorage.RavenTypeWatcherStorage.Clear();
        var raven = _ravenTypeWatcher.Object;
        var name = "TestWatcher";
        
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
        _ravenStorage.RavenWatcherStorage.TryAdd(name, raven);
        var mockedSubject = fixture.Create<TestSubjectEntity>();
        mockedSubject.Attach(raven);
        
        //Act
        sut.UpdateRavens(mockedSubject);
        
        //Assert
        _ravenWatcher.Verify(x => x.Update(mockedSubject));
        
        _ravenStorage.RavenWatcherStorage.Clear();
    }
}