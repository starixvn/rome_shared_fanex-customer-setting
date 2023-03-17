import React from "react";

const Message = ({ message }: { message: string; isError?: boolean }) => {
  return <div>{message}</div>;
};

export default Message;
