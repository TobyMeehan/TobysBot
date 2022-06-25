import React, {Component} from 'react';
import {FontAwesomeIcon} from '@fortawesome/react-fontawesome';
import {ConnectionStatus} from "../models/ConnectionStatus";

export class ConnectionState extends Component<{}, ConnectionStatus> {
    static displayName = ConnectionState.name;

    constructor(props: any) {
        super(props);
        this.state = {connectionState: "", numberOfGuilds: 0, loading: true}
    }

    async componentDidMount() {
        await this.getDiscordStatus();
    }

    renderConnectionState(connectionState: string, numberOfGuilds: number) {
        switch (connectionState.toLowerCase()) {
            case "connected":
                return (
                    <span className='lead'>

                        <FontAwesomeIcon icon="circle" color='green'/> 

                        {' '}

                        ONLINE

                        {' '}

                        <span className='text-muted'>
                            Online in {numberOfGuilds} servers.
                        </span>
                    </span>
                )
            case "connecting":
                return (
                    <span className='lead'>

                        <FontAwesomeIcon icon="circle" color='orange'/>

                        CONNECTING

                        <span className='text-muted'>
                            With you shortly!
                        </span>
                    </span>
                )
            case "disconnected":
            case "disconnecting":
                return (
                    <span className='lead'>
                        <FontAwesomeIcon icon="circle" color='red'/>
    
                        OFFLINE
    
                        <span className='text-muted'>
                            Oops! Somethings gone wrong and Toby's Bot is not online right now.
                        </span>
                    </span>
                )
        }
    }

    render() {
        let connectionState = this.state.loading
            ? <></>
            : this.renderConnectionState(this.state.connectionState, this.state.numberOfGuilds);

        return (
            <>
                {connectionState}
            </>
        );
    }

    async getDiscordStatus() {
        const response = await fetch('data/status');
        const data = await response.json();
        this.setState({connectionState: data.connectionState, numberOfGuilds: data.numberOfGuilds, loading: false})
    }
}