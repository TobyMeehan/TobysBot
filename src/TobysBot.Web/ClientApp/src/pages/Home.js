import React, {Component} from 'react';
import {ConnectionState} from '../components/ConnectionState';

export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);
        this.state = {status: {}, loading: true}
    }

    render() {
        return (
            <div>
                <div className="container-fluid discord-banner pb-5 pt-3">
                    <div className="container">
                        <div className="row">
                            <div className="col-md-5">
                                <h1 className="display-4">Hey!</h1>

                                <p>You've reached the homepage for my discord bot! Besides a range of random stuff
                                    Toby's
                                    Bot also has a
                                    suite of features for playing music and audio from YouTube, Spotify, Soundcloud and
                                    more.</p>

                                <p>To get started, use the button to invite the bot to a server, or continue reading for
                                    more
                                    information. Make sure you check out some of my other projects over at <a
                                        href="https://tobymeehan.com">tobymeehan.com</a></p>

                                <button className="btn btn-primary btn-lg">Add to Server</button>
                            </div>
                        </div>
                    </div>
                </div>

                <div className="container-fluid bg-600 py-3 mb-3">
                    <div className="container">
                        <p className="lead my-3">
                            <ConnectionState></ConnectionState>
                        </p>
                    </div>
                </div>

                <div className="container">
                    <h2>Music</h2>
                    <p>Toby's Bot has a huge range of features for listening to music. From queue management with
                        shuffle
                        and
                        repeat to integrated lyrics, as well as support for major formats including YouTube, Spotify and
                        Soundcloud,
                        your music listening needs are covered.</p>
                    <h2>Open Source</h2>
                    <p>Toby's Bot is completely free and open source software, under the GNU GPLv3. Check out the source
                        code on
                        Github for yourself!</p>
                    <h2>New Features Coming</h2>
                    <p>I am currently working on version 2 of Toby's Bot, adding a bunch of new features, as well as a
                        new
                        modular
                        and extensible design.</p>
                </div>
            </div>
        );
    }
}
    
