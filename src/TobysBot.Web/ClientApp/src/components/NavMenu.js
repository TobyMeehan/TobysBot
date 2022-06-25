import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import {FontAwesomeIcon} from '@fortawesome/react-fontawesome';
import './NavMenu.css';
import '../fonts.css';

import logo from '../img/discord.png';

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor (props) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = {
      collapsed: true
    };
  }

  toggleNavbar () {
    this.setState({
      collapsed: !this.state.collapsed
    });
  }

  render () {
    return (
      <header>
        <Navbar className="navbar-expand-sm navbar-toggleable-sm text-light box-shadow" dark>
          <Container>
            <NavbarBrand tag={Link} to="/">
              <img src={logo} width="30" height="30" className="d-inline-block align-top" alt="" />

              {' '}

              <span className='font-title'>
                TOBYSBOT
              </span>
            </NavbarBrand>
            <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
            <span className='mr-auto'></span>
            <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
              <ul className="navbar-nav ml-auto">
                <NavItem>
                  <NavLink tag={Link} to="/commands">
                    <FontAwesomeIcon icon="info-circle" />
                    {' '}
                    Commands
                  </NavLink>
                </NavItem>
                <NavItem>
                  <NavLink href="/invite">
                    <FontAwesomeIcon icon={['fab', 'discord']} />
                    {' '}
                    Invite
                  </NavLink>
                </NavItem>
                <NavItem>
                  <NavLink href="/github">
                    <FontAwesomeIcon icon={['fab', 'github']} />
                    {' '}
                    Github
                  </NavLink>
                </NavItem>
              </ul>
            </Collapse>
          </Container>
        </Navbar>
      </header>
    );
  }
}
