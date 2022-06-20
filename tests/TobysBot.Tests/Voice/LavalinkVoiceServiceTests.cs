using Discord;
using Moq;
using TobysBot.Voice.Lavalink;

namespace TobysBot.Tests.Voice;

public class LavalinkVoiceServiceTests
{
    private readonly LavalinkVoiceService _sut;
    
    private readonly Mock<ILavalinkNode> _nodeMock = new();
    private readonly Mock<ILavalinkPlayer> _playerMock = new();
    private readonly Mock<IVoiceChannel> _playerVoiceChannelMock = new();
    private readonly Mock<IGuild> _guildMock = new();
    private readonly Mock<IVoiceChannel> _voiceChannelMock = new();
    private readonly Mock<ITextChannel> _textChannelMock = new();

    public LavalinkVoiceServiceTests()
    {
        _sut = new LavalinkVoiceService(_nodeMock.Object);
    }
    
    [Fact]
    public async Task JoinAsync_ShouldJoinVoiceChannel_WhenNoPlayer()
    {
        // Arrange
        _nodeMock.Setup(x => x.GetPlayer(It.IsAny<IGuild>()))
            .Returns(() => null);
        
        var voiceChannel = _voiceChannelMock.Object;
        var textChannel = _textChannelMock.Object;
        
        // Act
        await _sut.JoinAsync(voiceChannel, textChannel);

        // Assert
        _nodeMock.Verify(x => x.JoinAsync(voiceChannel, textChannel), Times.Once);
    }

    [Fact]
    public async Task JoinAsync_ShouldJoinVoiceChannel_WhenNotConnected()
    {
        // Arrange
        _playerMock.SetupGet(x => x.IsConnected)
            .Returns(false);

        _nodeMock.Setup(x => x.GetPlayer(It.IsAny<IGuild>()))
            .Returns(_playerMock.Object);

        var voiceChannel = _voiceChannelMock.Object;
        var textChannel = _textChannelMock.Object;
        
        // Act
        await _sut.JoinAsync(voiceChannel, textChannel);
        
        // Assert
        _nodeMock.Verify(x => x.JoinAsync(voiceChannel, textChannel), Times.Once);
    }

    [Fact]
    public async Task JoinAsync_ShouldDoNothing_WhenInSameVoiceChannel()
    {
        // Arrange
        ulong voiceChannelId = 123456789;
        
        _playerVoiceChannelMock.SetupGet(x => x.Id)
            .Returns(voiceChannelId);
        
        _playerMock.SetupGet(x => x.IsConnected)
            .Returns(true);
        _playerMock.SetupGet(x => x.VoiceChannel)
            .Returns(_playerVoiceChannelMock.Object);

        _nodeMock.Setup(x => x.GetPlayer(It.IsAny<IGuild>()))
            .Returns(_playerMock.Object);

        _voiceChannelMock.SetupGet(x => x.Id)
            .Returns(voiceChannelId);

        var voiceChannel = _voiceChannelMock.Object;
        var textChannel = _textChannelMock.Object;
        
        // Act
        await _sut.JoinAsync(voiceChannel, textChannel);
        
        // Assert
        _nodeMock.Verify(x => x.JoinAsync(voiceChannel, textChannel), Times.Never);
        _nodeMock.Verify(x => x.MoveChannelAsync(voiceChannel), Times.Never);
    }

    [Fact]
    public async Task JoinAsync_ShouldMoveChannel_WhenAlreadyConnected()
    {
        // Arrange
        _playerMock.SetupGet(x => x.IsConnected)
            .Returns(true);

        _nodeMock.Setup(x => x.GetPlayer(It.IsAny<IGuild>()))
            .Returns(_playerMock.Object);

        var voiceChannel = _voiceChannelMock.Object;
        var textChannel = _textChannelMock.Object;
        
        // Act
        await _sut.JoinAsync(voiceChannel, textChannel);
        
        // Assert
        _nodeMock.Verify(x => x.MoveChannelAsync(voiceChannel), Times.Once);
    }

    [Fact]
    public async Task JoinAsync_ShouldMoveChannel_WhenInDifferentChannel()
    {
        // Arrange
        ulong voiceChannelId = 123456789;
        ulong playerVoiceChannelId = 987654321;
        
        _playerVoiceChannelMock.SetupGet(x => x.Id)
            .Returns(playerVoiceChannelId);
        
        _playerMock.SetupGet(x => x.IsConnected)
            .Returns(true);
        _playerMock.SetupGet(x => x.VoiceChannel)
            .Returns(_playerVoiceChannelMock.Object);

        _nodeMock.Setup(x => x.GetPlayer(It.IsAny<IGuild>()))
            .Returns(_playerMock.Object);

        _voiceChannelMock.SetupGet(x => x.Id)
            .Returns(voiceChannelId);

        var voiceChannel = _voiceChannelMock.Object;
        var textChannel = _textChannelMock.Object;
        
        // Act
        await _sut.JoinAsync(voiceChannel, textChannel);
        
        // Assert
        _nodeMock.Verify(x => x.MoveChannelAsync(voiceChannel), Times.Once);
    }

    [Fact]
    public async Task LeaveAsync_ShouldDoNothing_WhenNoPlayer()
    {
        // Arrange
        _nodeMock.Setup(x => x.GetPlayer(It.IsAny<IGuild>()))
            .Returns(() => null);

        var guild = _guildMock.Object;
        
        // Act
        await _sut.LeaveAsync(guild);

        // Assert
        _nodeMock.Verify(x => x.LeaveAsync(It.IsAny<IVoiceChannel>()), Times.Never);
    }

    [Fact]
    public async Task LeaveAsync_ShouldLeaveVoiceChannel_WhenGuildHasPlayer()
    {
        // Arrange
        _playerMock.SetupGet(x => x.VoiceChannel)
            .Returns(_playerVoiceChannelMock.Object);

        _nodeMock.Setup(x => x.GetPlayer(It.IsAny<IGuild>()))
            .Returns(_playerMock.Object);

        var guild = _guildMock.Object;
        
        // Act
        await _sut.LeaveAsync(guild);
        
        // Assert
        _nodeMock.Verify(x => x.LeaveAsync(_playerVoiceChannelMock.Object), Times.Once);
    }
}