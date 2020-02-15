import React, { Component } from 'react';
import Spinner from '../upload/Spinner'
import Buttons from '../upload/Buttons'
import '../App.css'

export class Upload extends Component {
    state = {
        uploading: false,
        images: [],
        message: ""
    }

    onChange = e => {
        const files = Array.from(e.target.files)
        this.setState({ uploading: true, message: "" })

        const formData = new FormData()

        files.forEach((file, i) => {
            formData.append(i, file)
        })
        fetch('api/Upload/PostImages',
            {
            method: 'POST',
                body: formData,
               headers: {
                   'Accept': 'multipart/form-data'
                },
        }
        )
            .then(
                res => {
                }
            )
            .then(images => {
                this.setState({
                    uploading: false,
                    images,
                    message: "Successfuly uploaded images"
                })
            })
    }

    render() {
        const { uploading, message } = this.state

        const content = () => {
            switch (true) {
                case uploading:
                    return <Spinner />
                default:
                    return <Buttons onChange={this.onChange} />
            }
        }

        return (
            <div>
                Select images for upload
                <div className='buttons'>
                    {content()}
                </div>

                <div>
                    {message}
                </div>
            </div>
        )
    }
}
//export default Upload;
