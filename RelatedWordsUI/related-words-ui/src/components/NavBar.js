import React, {Component} from 'react';
import { connect } from 'react-redux';
import {userSignOut} from '../actions/user';
import {newProject} from '../actions/projects';

class NavBar extends Component {

  signOut = (e) => {
    this.props.dispatch(userSignOut())
  }

  newProject = (e) => {
    this.props.dispatch(newProject(this.props.user.token));
  }

  render() {
    return (
      <div className="NavBar">
        <nav id="nav">
          <ul className="nav-bar nav-bar-right" id="right">

          </ul>
          <ul className="nav-bar nav-bar-left">
              <li><button  className="first">Projects  &raquo;</button>
                <ul>
                    <li><button onClick={this.newProject}>New Project</button></li>
                </ul>
              </li>
              <li><button>Filters</button></li>
              <li><button className="last">{this.props.user.email} &raquo;</button>
                <ul>
                    <li><button onClick={this.signOut}>Sign out</button></li>
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

export default connect(mapStateToProps)(NavBar);