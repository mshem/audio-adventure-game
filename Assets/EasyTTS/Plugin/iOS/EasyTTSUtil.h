//#import <Slt/Slt.h>
//#import <OpenEars/FliteController.h>
#import <AVFoundation/AVFoundation.h>

@interface EasyTTSUtil : UIViewController {
    //	FliteController *fliteController;
    //    Slt *slt;
    AVSpeechSynthesizer *synthesizer;
}
//@property (strong, nonatomic) FliteController *fliteController;
//@property (strong, nonatomic) Slt *slt;
@property (strong, nonatomic) AVSpeechSynthesizer *synthesizer;

@end

