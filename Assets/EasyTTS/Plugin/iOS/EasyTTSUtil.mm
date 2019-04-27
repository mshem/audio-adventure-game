#import "EasyTTSUtil.h"


@implementation EasyTTSUtil

@synthesize synthesizer;
- (AVSpeechSynthesizer *)synthesizer {
    if (synthesizer == nil) {
        synthesizer = [[AVSpeechSynthesizer alloc] init];
    }
    return synthesizer;
}

- (void) convertTextToSpeech:(NSString *)text local:(NSString*)local{
    AVSpeechUtterance *utterance = [[AVSpeechUtterance alloc] initWithString:text];
    NSString *os = [[UIDevice currentDevice] systemVersion];
    if ([os compare:@"9.0" options:NSNumericSearch] == NSOrderedAscending) {
        // ios9.0未満のとき
        utterance.rate = 0.12f;
    } else {
        // ios9.0以上のとき
        utterance.rate = 0.5f;
    }
    
    utterance.voice = [AVSpeechSynthesisVoice voiceWithLanguage:local];
    [self.synthesizer speakUtterance:utterance];
}

- (void) convertTextToSpeech:(NSString *)text local:(NSString*)local volume:(float)volume rate:(float)rate pitch:(float)pitch{
    AVSpeechUtterance *utterance = [[AVSpeechUtterance alloc] initWithString:text];
    NSString *os = [[UIDevice currentDevice] systemVersion];
    if(rate != 0){
        utterance.rate = rate;
    }else{
        if ([os compare:@"9.0" options:NSNumericSearch] == NSOrderedAscending) {
            utterance.rate = 0.12f;
        } else {
            utterance.rate = 0.5f;
        }
    }
    if(volume!=0){
        utterance.volume = volume;
    }
    
    if(pitch!=0){
        utterance.pitchMultiplier = pitch;
    }
    
    utterance.voice = [AVSpeechSynthesisVoice voiceWithLanguage:local];
    [self.synthesizer speakUtterance:utterance];
}

- (void) stopSpeech {
    [self.synthesizer stopSpeakingAtBoundary:AVSpeechBoundaryImmediate];
}

@end

extern "C" {
    void EasyTTSUtilSpeechAd(char* text,char* local,float volume,float rate,float pitch);
    void EasyTTSUtilSpeech(char* text,char* local);
    void EasyTTSUtilStop();
    
}


static EasyTTSUtil *tts = nil;


void EasyTTSUtilSpeech(char* text,char* _local)
{
    if(tts == nil)
    {
        tts = [EasyTTSUtil alloc];
    }
    NSString *sName = (text != nil) ? [NSString stringWithUTF8String: text] : [NSString stringWithUTF8String: ""];
    NSString *slocal = (text != nil) ? [NSString stringWithUTF8String: _local] : [NSString stringWithUTF8String: ""];
    [tts convertTextToSpeech:sName local:slocal];
}

void EasyTTSUtilSpeechAd(char* text,char* _local,float volume,float rate,float pitch)
{
    if(tts == nil)
    {
        tts = [EasyTTSUtil alloc];
    }
    NSString *sName = (text != nil) ? [NSString stringWithUTF8String: text] : [NSString stringWithUTF8String: ""];
    NSString *slocal = (text != nil) ? [NSString stringWithUTF8String: _local] : [NSString stringWithUTF8String: ""];
    [tts convertTextToSpeech:sName local:slocal volume:volume rate:rate pitch:pitch];
}

void EasyTTSUtilStop()
{
    if(tts != nil)
    {
        [tts stopSpeech];
    }
    
}
