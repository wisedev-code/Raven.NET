using System.Collections.Concurrent;
using AutoFixture;
using Moq;
using Raven.NET.Core.Exceptions;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Providers;
using Raven.NET.Core.Providers.Interfaces;
using Raven.NET.Core.Static;
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
    
    public RavenProviderTests()
    {
        _ravenWatcher = new Mock<IRavenWatcher>();
        _ravenTypeWatcher = new Mock<IRavenTypeWatcher>();
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
        RavenCache.RavenWatcherCache.ShouldContainKey(name);
        
        RavenCache.RavenWatcherCache.Clear();
    }
    
    [Fact]
    void AddRaven_Should_ThrowException_WhenRavenAlreadyExists()
    {
        //Arrange 
        var raven = _ravenWatcher.Object;
        var name = "TestWatcher";
        RavenCache.RavenWatcherCache.TryAdd(name, raven);

        //Act
        var ex = Record.Exception(() => sut.AddRaven(name, raven));
        
        //Assert
        ex.ShouldNotBeNull();
        ex.ShouldBeOfType<RavenAlreadyExistsException>();
        
        RavenCache.RavenWatcherCache.Clear();

    }
    
    [Fact]
    void AddRaven_Should_AddRavenToInternalRavenTypeCache_WhenTypeProvided()
    {
        //Arrange 
        var raven = _ravenTypeWatcher.Object;
        var name = "TestWatcher";

        //Act
        sut.AddRaven(name, raven, typeof(TestSubjectEntity));
        
        //Assert
        RavenCache.RavenTypeWatcherCache.ShouldContainKey(typeof(TestSubjectEntity));
        
        RavenCache.RavenTypeWatcherCache.Clear();
    }
    
    [Fact]
    void AddRaven_Should_ThrowException_WhenRavenTypeAlreadyExists()
    {
        //Arrange 
        var raven = _ravenTypeWatcher.Object;
        var name = "TestWatcher";
        RavenCache.RavenTypeWatcherCache.TryAdd(typeof(TestSubjectEntity), raven);

        //Act
        var ex = Record.Exception(() => sut.AddRaven(name, raven, typeof(TestSubjectEntity)));
        
        //Assert
        ex.ShouldNotBeNull();
        ex.ShouldBeOfType<RavenForTypeAlreadyExistsException>();
        
        RavenCache.RavenTypeWatcherCache.Clear();
    }
    
    [Fact]
    void RemoveRaven_Should_RemoveRavenFromInternalCache()
    {
        //Arrange 
        var raven = _ravenWatcher.Object;
        var name = "TestWatcher";
        RavenCache.RavenWatcherCache.TryAdd(name, raven);

        //Act
        sut.RemoveRaven(name);
        
        //Assert
        RavenCache.RavenWatcherCache.ShouldBeEmpty();
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
        RavenCache.RavenWatcherCache.TryAdd(name, raven);
        
        //Act
        var result = sut.GetRaven(name);
        
        //Assert
        result.ShouldNotBeNull();
        result.ShouldBeEquivalentTo(raven);
        
        RavenCache.RavenWatcherCache.Clear();
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
        RavenCache.RavenTypeWatcherCache.TryAdd(typeof(TestSubjectEntity), raven);
        
        //Act
        var result = sut.GetRaven(name, typeof(TestSubjectEntity));
        
        //Assert
        result.ShouldNotBeNull();
        result.ShouldBeEquivalentTo(raven);
        
        RavenCache.RavenTypeWatcherCache.Clear();
    }
    
    [Fact]
    void GetRaven_Should_ReturnException_WhenTypeRavenDoesNotExist()
    {
        //Arrange 
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
        RavenCache.RavenWatcherCache.TryAdd(name, raven);
        var mockedSubject = fixture.Create<TestSubjectEntity>();
        mockedSubject.Attach(raven);
        
        //Act
        sut.UpdateRavens(mockedSubject);
        
        //Assert
        _ravenWatcher.Verify(x => x.Update(mockedSubject));
        
        RavenCache.RavenWatcherCache.Clear();
    }
}