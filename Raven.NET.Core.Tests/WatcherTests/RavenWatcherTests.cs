using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.NET.Core.Configuration;
using Raven.NET.Core.Exceptions;
using Raven.NET.Core.Observers;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Providers.Interfaces;
using Raven.NET.Core.Storage;
using Raven.NET.Core.Storage.Interfaces;
using Raven.NET.Core.Subjects;
using Raven.NET.Core.Tests.SubjectTests;
using Shouldly;
using Xunit;

namespace Raven.NET.Core.Tests.WatcherTests;

[Collection("Tests")]
public class RavenWatcherTests
{
    private Fixture _fixture;
    private readonly IRavenWatcher sut;
    private readonly IRavenStorage _ravenStorage;
    private readonly Mock<IRavenProvider> _ravenProvider;
    private readonly Mock<IRavenSettingsProvider> _ravenSettingsProvider;

    public RavenWatcherTests()
    {
        _fixture = new Fixture();
        _ravenProvider = new Mock<IRavenProvider>();
        _ravenSettingsProvider = new Mock<IRavenSettingsProvider>();
        sut = new RavenWatcher(_ravenProvider.Object, _ravenSettingsProvider.Object);
        _ravenStorage = RavenStorage.Instance;
    }

    [Fact]
    void Create_Should_AddWatcherToCacheWithDefaultConfiguration_When_NoConfigFileSpecified()
    {
        //Arrange
        var watcherName = "TestWatcher";
        _ravenProvider.Setup(x => x.AddRaven(It.IsAny<string>(), It.IsAny<IRavenWatcher>(), null)).Callback(() =>
        {
            _ravenStorage.RavenWatcherTryAdd(watcherName, sut);
        });

        //Act
        sut.Create(watcherName, subject => true);

        //Assert
        _ravenStorage.RavenWatcherExists(watcherName).ShouldBeTrue();
        var settings = (sut as RavenWatcher)?._ravenSettings;
        settings.LogLevel.ShouldBe(LogLevel.Warning);
        settings.AutoDestroy.ShouldBe(false);
        _ravenProvider.Verify(x => x.AddRaven(watcherName, sut, null));
        _ravenSettingsProvider.Verify(x => x.GetRaven(watcherName));
    }

    [Fact]
    void Create_Should_AddWatcherToCacheWithFileConfiguration_When_ConfigFileSpecified()
    {
        //Arrange
        var watcherName = "TestWatcher";
        var settingsMock = _fixture.Create<RavenSettings>();
        settingsMock.AutoDestroy = true;
        settingsMock.LogLevel = LogLevel.Trace;

        _ravenProvider.Setup(x => x.AddRaven(It.IsAny<string>(), It.IsAny<IRavenWatcher>(), null)).Callback(() =>
        {
            _ravenStorage.RavenWatcherTryAdd(watcherName, sut);
        });

        _ravenSettingsProvider.Setup(x => x.GetRaven(watcherName)).Returns(settingsMock);

        //Act
        sut.Create(watcherName, subject => true);

        //Assert
        _ravenStorage.RavenWatcherExists(watcherName).ShouldBeTrue();
        var settings = (sut as RavenWatcher)?._ravenSettings;
        settings.LogLevel.ShouldBe(LogLevel.Trace);
        settings.AutoDestroy.ShouldBe(true);
        _ravenSettingsProvider.Verify(x => x.GetRaven(watcherName));
        _ravenProvider.Verify(x => x.AddRaven(watcherName, sut, null));
    }

    [Fact]
    void Create_Should_AddWatcherToCacheWithProvidedConfiguration_When_OptionsParameterProvided()
    {
        //Arrange
        var watcherName = "TestWatcher";
        var settingsMock = _fixture.Create<RavenSettings>();
        settingsMock.AutoDestroy = true;
        settingsMock.LogLevel = LogLevel.Trace;

        _ravenProvider.Setup(x => x.AddRaven(It.IsAny<string>(), It.IsAny<IRavenWatcher>(), null)).Callback(() =>
        {
            _ravenStorage.RavenWatcherTryAdd(watcherName, sut);
        });

        _ravenSettingsProvider.Setup(x => x.GetRaven(watcherName)).Returns(settingsMock);

        //Act
        sut.Create(watcherName, subject => true, ravenSettings =>
        {
            ravenSettings.AutoDestroy = false;
            ravenSettings.LogLevel = LogLevel.Critical;
        });

        //Assert
        _ravenStorage.RavenWatcherExists(watcherName).ShouldBeTrue();
        var settings = (sut as RavenWatcher)?._ravenSettings;
        settings.LogLevel.ShouldBe(LogLevel.Critical);
        settings.AutoDestroy.ShouldBe(false);
        _ravenSettingsProvider.Verify(x => x.GetRaven(watcherName));
        _ravenProvider.Verify(x => x.AddRaven(watcherName, sut, null));
    }

    [Fact]
    void Watch_Should_AddSubjectToInternalCollection()
    {
        //Arrange
        var watcherName = "TestSubject";
        var mockedSubject = new TestSubjectEntity();
        sut.Create(watcherName, subject => true);

        //Act
        sut.Watch(mockedSubject);

        //Assert
        var watcher = sut as RavenWatcher;
        watcher._watchedSubjects.ShouldContain(mockedSubject);
        mockedSubject.Observers.ShouldContain(watcher);
    }

    [Fact]
    void Watch_Should_ThrowException_When_SubjectAlreadyWatched()
    {
        //Arrange
        var watcherName = "TestSubject";
        var mockedSubject = new TestSubjectEntity();
        sut.Create(watcherName, subject => true);

        //Act
        var ex = Record.Exception(() => sut.Watch(mockedSubject).Watch(mockedSubject));

        //Assert
        ex.ShouldNotBeNull();
        ex.ShouldBeOfType<SubjectAlreadyWatchedException>();
    }

    [Fact]
    void UnWatch_Should_RemoveSubjectFromInternalCollection()
    {
        //Arrange
        var watcherName = "TestSubject";
        var mockedSubject = new TestSubjectEntity();
        sut.Create(watcherName, subject => true);
        sut.Watch(mockedSubject);
        _ravenProvider.Setup(x => x.GetRaven(watcherName, null)).Returns(sut as RavenWatcher);
        _ravenProvider.Setup(x => x.UpdateSubjects(It.IsAny<string>(), It.IsAny<IEnumerable<RavenSubject>>(), null))
            .Callback(
                () => { mockedSubject.Observers.Clear(); });

        //Act
        sut.UnWatch(watcherName, mockedSubject);

        //Assert
        var watcher = sut as RavenWatcher;
        watcher._watchedSubjects.ShouldNotContain(mockedSubject);
        mockedSubject.Observers.ShouldNotContain(watcher);
        _ravenProvider.Verify(x => x.UpdateSubjects(watcherName, It.IsAny<IEnumerable<RavenSubject>>(), null));
    }

    [Fact]
    void UnWatch_Should_ThrowException_WhenRavenDoesNotExists()
    {
        //Arrange
        var watcherName = "TestSubjectFake";
        var mockedSubject = new TestSubjectEntity();
        sut.Create(watcherName, subject => true);
        sut.Watch(mockedSubject);

        //Act
        var ex = Record.Exception(() => sut.UnWatch(watcherName, mockedSubject));

        //Assert
        ex.ShouldNotBeNull();
        ex.ShouldBeOfType<RavenDoesNotExistsException>();
    }

    [Fact]
    void Stop_Should_RemoveAllSubjectsFromInternalCollection()
    {
        //Arrange
        var watcherName = "TestSubject";
        var mockedSubject1 = new TestSubjectEntity();
        var mockedSubject2 = new TestSubjectEntity();
        sut.Create(watcherName, subject => true);
        sut.Watch(mockedSubject1).Watch(mockedSubject2);
        _ravenProvider.Setup(x => x.GetRaven(watcherName, null)).Returns(sut as RavenWatcher);
        _ravenProvider.Setup(x => x.UpdateSubjects(It.IsAny<string>(), It.IsAny<IEnumerable<RavenSubject>>(), null))
            .Callback(
                () =>
                {
                    mockedSubject1.Observers.Clear();
                    mockedSubject2.Observers.Clear();
                });

        //Act
        sut.Stop(watcherName);

        //Assert
        var watcher = sut as RavenWatcher;
        watcher._watchedSubjects.ShouldNotContain(mockedSubject1);
        watcher._watchedSubjects.ShouldNotContain(mockedSubject2);
        mockedSubject1.Observers.ShouldNotContain(watcher);
        mockedSubject2.Observers.ShouldNotContain(watcher);
        _ravenProvider.Verify(x => x.UpdateSubjects(watcherName, It.IsAny<IEnumerable<RavenSubject>>(), null));
    }
}