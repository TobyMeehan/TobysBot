import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './shared/Layout';
import { Home } from './pages/Home';
import { FetchData } from './components/FetchData';
import { Counter } from './components/Counter';
import { library } from '@fortawesome/fontawesome-svg-core'
import { fab } from '@fortawesome/free-brands-svg-icons'
import { fas } from '@fortawesome/free-solid-svg-icons'

import './custom.css'

library.add(fab, fas)

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/counter' component={Counter} />
        <Route path='/fetch-data' component={FetchData} />
      </Layout>
    );
  }
}
