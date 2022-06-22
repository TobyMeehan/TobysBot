import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';

export class Layout extends Component {
  static displayName = Layout.name;

  render () {
    return (
      <div>
        <NavMenu />

        <main role="main" className="pb-3 text-light">
          {this.props.children}
        </main>

        <footer className='footer text-muted bg-dark'>
          <Container>
            &copy; {new Date().getFullYear()} - Toby Meehan
          </Container>
        </footer>
      </div>
    );
  }
}
