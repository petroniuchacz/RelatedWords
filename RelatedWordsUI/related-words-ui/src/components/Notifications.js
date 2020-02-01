import React, { Component } from "react";
import { connect } from "react-redux";
import Notification from "./Notification";
import {deleteNotification} from '../actions/notifications'

class Notifications extends Component {

  deleteNotification = (e, id) => {
    this.props.dispatch(deleteNotification(id));
  }

  render() {
    return (
      <div id="notifications">
        {this.props.notifications.map(notification => 
          <Notification 
            key={notification.id}
            notification={notification}
            deleteNotification={this.deleteNotification}
          />
        )}
      </div>
    )
  }
}

function mapStateToProps(state) {               
  return {
    notifications: state.notifications                    
  }
}

export default connect(mapStateToProps)(Notifications);