import React from "react";
import Modal from "react-modal";

const customStyles = {
  content: {
    top: "50%",
    left: "50%",
    right: "auto",
    bottom: "auto",
    marginRight: "-50%",
    transform: "translate(-50%, -50%)",
  },
};

Modal.setAppElement("#main");

export const BasicModal = ({ isOpen, handleRequestClose, data }: any) => {
  return (
    <>
      <Modal
        isOpen={isOpen}
        style={customStyles}
        onRequestClose={handleRequestClose}
        
      >
        <div>{data}</div>
      </Modal>
    </>
  );
};
