import React from 'react';

const Notification = (props) => {
  const {notification} = props;
  const notificationId = notification.id;
  return (
    <p 
      id={notificationId} 
      className={"notification notification-" + notification.type}
      onMouseLeave={(e) => props.deleteNotification(e, notificationId)}
    >
      <span className="notification-message">
        {notification.message}
      </span>
    </p>
  )
}

export default Notification;