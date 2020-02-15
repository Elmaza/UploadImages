import React, { Component } from 'react';
import Gallery from 'react-grid-gallery';
import '../App.css'
export class FetchData extends Component {
    constructor(props) {
        super(props);

        this.state = {
            images: this.props.images,
            currentImage: 0,
            isLoaded: false
        };

        this.onCurrentImageChange = this.onCurrentImageChange.bind(this);
        this.deleteImage = this.deleteImage.bind(this);
    }

    onCurrentImageChange(index) {
        this.setState({ currentImage: index });
    }

    deleteImage() {
        if (window.confirm(`Are you sure you want to delete image number ${this.state.currentImage}?`)) {
            fetch(`api/Upload/DeleteImage/${this.state.images[this.state.currentImage].id}`, {
                method: 'DELETE'
            })
            var images = this.state.images.slice();
            images.splice(this.state.currentImage, 1)
            this.setState({
                images: images
            });
        }
    }
    componentDidMount() {
        fetch('api/Upload/GetImages')
            .then(res => {
                //debugger;
                res.json().then(r => {
                    //console.log(r)
                    //debugger;
                    this.setState({
                        isLoaded: true,
                        images: r
                    })
                })
            })

    }
    render() {
        const { error, isLoaded, images, currentImage } = this.state;
        if (error) {
            return <div>Error: {error.message}</div>;
        } else if (!isLoaded) {
            return <div class="lds-hourglass">Loading...</div>;
        } else {
            return (
                <div style={{
                    display: "block",
                    minHeight: "1px",
                    width: "100%",
                    border: "1px solid #ddd",
                    overflow: "auto"
                }}>
                    <div style={{
                        padding: "2px",
                        color: "#666"
                    }}>Current image: {currentImage}</div>
                    <Gallery
                        images={images}
                        enableLightbox={true}
                        enableImageSelection={false}
                        currentImageWillChange={this.onCurrentImageChange}

                        customControls={[
                            <button key="deleteImage" onClick={this.deleteImage}>Delete Image</button>
                        ]}
                    />
                </div>
            );
        }
    }
}
