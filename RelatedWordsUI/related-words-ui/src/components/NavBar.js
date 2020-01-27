import React, {Component} from 'react';
import { connect } from 'react-redux';

class NavBar extends Component {

  render() {
    return (
      <div className="NavBar">
        <nav id="nav">
          <ul className="nav-bar nav-bar-right" id="right">
            <li><button>{this.props.user.email} &raquo;</button>
                <ul>
                    <li><button>Logout</button></li>
                </ul>
            </li>
          </ul>
          <ul className="nav-bar nav-bar-left">
              <li><button  className="first">Projects</button></li>
              <li><button  className="first">Filters</button></li>
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