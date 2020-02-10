import React, {Component} from 'react';
import { connect } from 'react-redux';
import {withRouter} from 'react-router-dom';
import {userSignOut} from '../actions/user';
import {newProject} from '../actions/projects';
import {uuid} from 'uuidv4';

class NavBar extends Component {

  constructor(props) {
    super(props);
    this.state = {
      openProjectMenu: false
    }
  }

  signOut = (e) => {
    this.props.dispatch(userSignOut())
  }

  newProject = (e) => {
    this.props.dispatch(newProject(this.props.user.token));
  }

  openProjectMenu = e => {
    this.props.history.push(`${this.props.currentPath}/projectmenu`)
  }

  render() {
    return (
      <div className="NavBar">
        <nav id="nav">
          <ul  className="nav-bar nav-bar-right" id="right" key={`${uuid()}`}>

          </ul>
          <ul className="nav-bar nav-bar-left" key={`${uuid()}`}>
              <li key={`${uuid()}`}><button  className="first">Projects  &raquo;</button>
                <ul>
                    <li key={`${uuid()}`}><button onClick={this.newProject}>New Project</button></li>
                    <li key={`${uuid()}`}><button onClick={this.openProjectMenu}>Menu</button></li>
                </ul>
              </li>
              <li key={`${uuid()}`}><button>Filters</button></li>
              <li key={`${uuid()}`}><button className="last">{this.props.user.email} &raquo;</button>
                <ul>
                    <li key={`${uuid()}`}><button onClick={this.signOut}>Sign out</button></li>
                </ul>
            </li>
          </ul>
        </nav>
      </div>
    )
  }
}

function mapStateToProps(state) {               
  return {
    user: state.user.user,
  }
}

export default withRouter(connect(mapStateToProps)(NavBar));