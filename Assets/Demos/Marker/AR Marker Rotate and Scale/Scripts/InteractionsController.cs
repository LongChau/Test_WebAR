using UnityEngine;

namespace ARWT.Example{
    public class InteractionsController : MonoBehaviour{

        public float scaleMultiplier = .01f;
        public float minScale = .1f;
        public float maxScale = 3f;

        public float angleTreshold = .2f;
        public float scaleTreshold = .002f;

        Vector2 startPosition;
        float startDistance;
        bool interaciting = false;

        void Update() {
            // Only active when there is 2 touch count.
            if(Input.touchCount == 2){

                // get first touch.
                var t0 = Input.GetTouch(0);
                // get second touch.
                var t1 = Input.GetTouch(1);


                if(!interaciting){
                    // Start interacting. Run once at touching phase.
                    startPosition = t1.position - t0.position;
                    startDistance = Vector2.Distance(t1.position, t0.position);
                    interaciting = true;
                }else{
                    // When interacting...
                    // Update the current position.
                    Vector2 currPosition = t1.position - t0.position;
                    // Find the angle offset between start and current point.
                    float angleOffset = Vector2.Angle(startPosition, currPosition);
                    // Check the perdincular vector to rotate the object later.
                    Vector3 cross = Vector3.Cross(startPosition, currPosition);
                    
                    // Check the current angle with threshold.
                    if(angleOffset > angleTreshold){
                        startPosition = currPosition;
                        
                        // Check perdincular vector to rotate.
                        if (cross.z > 0) {
                            transform.RotateAround(transform.position, transform.up, -angleOffset);
                        } else if (cross.z < 0) {
                            transform.RotateAround(transform.position, transform.up, angleOffset);
                        }
                    }
                    
                    // check the distance to scale.
                    float currentDistance = Vector2.Distance(t1.position, t0.position);
                    float scalingAmount = (currentDistance - startDistance) * scaleMultiplier;

                    if(Mathf.Abs(scalingAmount) > Mathf.Abs(scaleTreshold)){
                        startDistance = currentDistance;
                        
                        // Scale the object but does not "cross the line".
                        Vector3 newScale = new Vector3(
                            Mathf.Clamp(transform.localScale.x + scalingAmount, minScale, maxScale),
                            Mathf.Clamp(transform.localScale.y + scalingAmount, minScale, maxScale),
                            Mathf.Clamp(transform.localScale.z + scalingAmount, minScale, maxScale)    
                        );
                        transform.localScale = newScale;
                    }
                }

            }else{
                // When there is no input. Stop the interaction.
                interaciting = false;
            }
        }
    }
}
