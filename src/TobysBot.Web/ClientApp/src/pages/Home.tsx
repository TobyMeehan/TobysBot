import React, {Component} from 'react';
import {ConnectionState} from '../components/ConnectionState';
import {Container} from "reactstrap";
import {Helmet} from "react-helmet";

import './Home.css';

export class Home extends Component {
    static displayName = Home.name;

    constructor(props: any) {
        super(props);
        this.state = {status: {}, loading: true}
    }

    render() {
        return (
            <>
                <Helmet>
                    <title>Home</title>
                </Helmet>
                <div>
                    <div className="container-fluid discord-banner pb-5 pt-3">
                        <Container>
                            <div className="row">
                                <div className="col-md-5">
                                    <h1 className="display-4">Hey!</h1>

                                    <p>You've reached the homepage for Toby's Bot! Now on v2.0, this project aims to be a
                                        comprehensive open source discord bot with many of the features of other popular
                                        bots while having no paywalls or premium tier.</p>

                                    <p>To get started, use the button to invite the bot to a server, or continue reading for
                                        more information. Make sure you check out some of my other projects over at <a
                                            href="https://tobymeehan.com">tobymeehan.com</a></p>

                                    <a href="/invite" className="btn btn-primary btn-lg">Add to Server</a>
                                </div>
                            </div>
                        </Container>
                    </div>

                    <div className="container-fluid bg-600 py-3 mb-3">
                        <div className="container">
                            <p className="lead my-3">
                                <ConnectionState></ConnectionState>
                            </p>
                        </div>
                    </div>

                    <div className="container">
                        <div className='col-md-8'>
                            <h2>Music</h2>
                            <p>Toby's Bot comes with everything you'd expect from a music bot, including queue management, looping, shuffle and lyrics.</p>
                            <h2>No Premium</h2>
                            <p>There is not and will never be a premium tier, all features are free to use for everyone.
                                Also many put behind paywalls by certain other bots, such as audio effects and saved music queues
                                are available with Toby's Bot free of charge.</p>
                            <h2>Open Source</h2>
                            <p>Toby's Bot is free and open source software, licensed under the AGPL. All the code is available
                                to view on <a href='/github'>Github</a> so if you're wondering how something works, go have a
                                look for yourself! I wouldn't mind a pull request or two either...</p>
                            <h2>New Features Coming</h2>
                            <p>Version 2 recently released, bringing a range of new features including slash commands, audio effects
                                and a complete redesign of the codebase, meaning adding new functionality is significantly easier.
                                Even more plugins are planned to be added, including moderation, custom commands and an economy.</p>
                        </div>
                    </div>
                </div>
            </>
        );
    }
}
    

