import React, {Component} from 'react';
import {Route} from 'react-router';
import {Layout} from './shared/Layout';
import {Home} from './pages/Home';
import {Commands} from "./pages/Commands";
import {library} from '@fortawesome/fontawesome-svg-core'
import {fab} from '@fortawesome/free-brands-svg-icons'
import {fas} from '@fortawesome/free-solid-svg-icons'
import {Helmet} from "react-helmet";

import './custom.css'

library.add(fab, fas)

export default class App extends Component {
    static displayName = App.name;

    render() {
        return (
            <>
                <Helmet defaultTitle="Toby's Bot" titleTemplate="%s · Toby's Bot">
                    <link rel="apple-touch-icon" sizes="180x180" href="/apple-touch-icon.png"/>
                    <link rel="icon" type="image/png" sizes="32x32" href="/favicon-32x32.png"/>
                    <link rel="icon" type="image/png" sizes="192x192" href="/android-chrome-192x192.png"/>
                    <link rel="icon" type="image/png" sizes="16x16" href="/favicon-16x16.png"/>
                    <link rel="manifest" href="/site.webmanifest"/>
                    <link rel="mask-icon" href="/safari-pinned-tab.svg" color="#32072c"/>
                    <meta name="apple-mobile-web-app-title" content="Toby's Bot"/>
                    <meta name="application-name" content="Toby's Bot"/>
                    <meta name="msapplication-TileColor" content="#32072c"/>
                    <meta name="theme-color" content="#32072c"/>
                </Helmet>

                <Layout>
                    <Route exact path='/' component={Home}/>
                    <Route path='/commands' component={Commands}/>
                </Layout>
            </>
        );
    }
}
