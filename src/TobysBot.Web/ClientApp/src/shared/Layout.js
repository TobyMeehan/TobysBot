import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from '../components/NavMenu';

export class Layout extends Component {
  static displayName = Layout.name;

  render () {
    return (
      <div>
        <NavMenu />

        <main role="main" className="pb-3 text-light">
          {this.props.children}
        </main>

        <footer className='footer text-muted bg-600'>
          <Container>
            &copy; {new Date().getFullYear()} - Toby Meehan
          </Container>
        </footer>
      </div>
    );
  }
}
