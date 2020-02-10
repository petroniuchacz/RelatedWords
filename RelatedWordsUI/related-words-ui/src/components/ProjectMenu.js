import React, {Component, Fragment} from 'react';
import {getUserProjects} from '../actions/projects';
import {deleteProject}from '../actions/projects';
import { connect } from "react-redux";
import { uuid } from 'uuidv4';
import _ from 'lodash';

const sortAttr = ['name', 'projectId', 'processingStatus', 'createdDate'];

class ProjectMenu extends Component {
  constructor(props) {
    super(props);
    this.state = {
      sortedBy: sortAttr[1]
    }
  }

  componentDidMount() {
    this.props.dispatch(getUserProjects(this.props.user.token));
  }

  sortBy(attrNum) {
    this.setState({sortedBy: sortAttr[attrNum]});
  }

  delProject(projectId) {
    this.props.dispatch(deleteProject(this.props.user.token, projectId))
  }

  openProject(projectId) {
    this.props.history.push(`${this.props.currentPath}/../project/${projectId}`)
  }

  render () {
    return (
      <div id="project-menu">
        <div className="project-menu-top" key={`${uuid()}`}
          onClick={() => this.sortBy(0)}
        >
          Name
        </div>
        <div className="project-menu-top" key={`${uuid()}`}
          onClick={() => this.sortBy(1)}
        >
          ID
        </div>
        <div className="project-menu-top" key={`${uuid()}`}
          onClick={() => this.sortBy(2)}
        >
          Processing
        </div>
        <div className="project-menu-top project-menu-top-last" key={`${uuid()}`}
          onClick={() => this.sortBy(3)}
        >
          Created
        </div>
        <div key={`${uuid()}`}></div>
        <div key={`${uuid()}`}></div>
        {_.sortBy(this.props.projects, [this.state.sortedBy]).map(p => 
          <Fragment>
            <div key={`${uuid()}`}>{p.name}</div>
            <div key={`${uuid()}`}>{p.projectId}</div>
            <div key={`${uuid()}`}>{p.processingStatusToString()}</div>
            <div key={`${uuid()}`}>{p.createdDate.split('.')[0]}</div>
            <div key={`${uuid()}`}>
              <button key={`${uuid()}`}
              onClick={() => this.openProject(p.projectId)} 
              className="button-green">
                Open
              </button>
            </div>
            <div key={`${uuid()}`}>
              <button key={`${uuid()}`} 
              onClick={() => this.delProject(p.projectId)} 
              className="button-red">
                Delete
              </button>
            </div>
          </Fragment>
        )}
      </div>
    )
  }
}

function mapStateToProps(state) {               
  return {
    projects: state.projects.projects,
    user: state.user.user                         
  }
}

export default connect(mapStateToProps)(ProjectMenu);